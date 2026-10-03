using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Application.Products.Queries;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Queries.Products
{
    public sealed class ProductCatalogReader(AppDbContext db) : IProductCatalogReader
    {
        private async Task<Store> Owner(Guid storeId, Guid userId, CancellationToken ct) =>
            await db.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.Id == storeId && x.UserId == userId, ct) ?? throw new NotFoundException("Store was not found.");
        private IQueryable<Product> Products(Guid storeId) => db.Products.AsNoTracking().Where(x => x.StoreId == storeId)
            .Include(x => x.Options).ThenInclude(x => x.Values)
            .Include(x => x.Variants).ThenInclude(x => x.Selections).Include(x => x.Images).Include(x => x.DeliveryOptions).AsSplitQuery();
        private Task<Dictionary<Guid, InventoryItem>> Stock(IEnumerable<Product> products, CancellationToken ct)
        {
            var ids = products.SelectMany(x => x.Variants).Select(x => x.Id).ToArray();
            return db.Set<InventoryItem>().AsNoTracking().Where(x => ids.Contains(x.ProductVariantId)).ToDictionaryAsync(x => x.ProductVariantId, ct);
        }
        public async Task<ProductEditorDataDto> EditorDataAsync(Guid storeId, Guid userId, CancellationToken ct)
        {
            var store = await Owner(storeId, userId, ct);
            var categories = await db.Set<StoreProductCategory>().AsNoTracking().Where(x => x.StoreId == storeId).OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToListAsync(ct);
            // Keep ancestors and currently referenced platform categories available to the picker.
            var platform = await db.Categories.AsNoTracking().OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).ToListAsync(ct);
            var options = await db.Set<StoreDeliveryOption>().AsNoTracking().Where(x => x.StoreId == storeId).OrderBy(x => x.Name).ToListAsync(ct);
            var country = store.Location.CountryCode;
            return new(new(store.Id, store.Name, store.Status == StoreStatus.Active ? "Published" : store.Status.ToString(), store.Slug,
                country, store.LogoUrl, store.CoverImageUrl, store.IsPickupAvailable, store.IsDeliveryAvailable),
                country == "AU" ? "AUD" : "BDT", categories.Select(StoreCategoryDto.From).ToList(),
                platform.Select(x => new MarketplaceCategoryDto(x.Id, x.ParentCategoryId,
                    x.Status == CategoryStatus.Active ? x.Name : x.Name + " (unavailable)", x.DisplayOrder)).ToList(), options.Select(DeliveryOptionDto.From).ToList());
        }
        public async Task<SellerProductDto> GetAsync(Guid storeId, Guid userId, Guid productId, CancellationToken ct)
        {
            await Owner(storeId, userId, ct);
            var product = await Products(storeId).SingleOrDefaultAsync(x => x.Id == productId, ct) ?? throw new NotFoundException("Product was not found.");
            return SellerProductDto.From(product, await Stock([product], ct));
        }
        public async Task<ProductPageDto> ListAsync(Guid storeId, Guid userId, ProductListFilter filter, CancellationToken ct)
        {
            await Owner(storeId, userId, ct);
            if (filter.Page < 1 || filter.Page > 1000000 || filter.PageSize < 1 || filter.PageSize > 100) throw new DomainException("Invalid page or page size.");
            if ((filter.Search?.Length ?? 0) > 150) throw new DomainException("Search text is too long.");
            var query = Products(storeId);
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLowerInvariant();
                query = query.Where(x => x.Name.ToLower().Contains(search));
            }
            query = filter.Status switch
            {
                "Current" => query.Where(x => x.Status != ProductStatus.Archived),
                "Published" => query.Where(x => x.Status == ProductStatus.Active),
                "Draft" => query.Where(x => x.Status == ProductStatus.Draft),
                "Archived" => query.Where(x => x.Status == ProductStatus.Archived),
                _ => throw new DomainException("Invalid product status filter.")
            };
            if (filter.CategoryId.HasValue)
            {
                var nodes = await db.Set<StoreProductCategory>().AsNoTracking().Where(x => x.StoreId == storeId).Select(x => new { x.Id, x.ParentId }).ToListAsync(ct);
                if (!nodes.Any(x => x.Id == filter.CategoryId.Value)) throw new DomainException("The category does not belong to this store.");
                var ids = CatalogRules.Descendants(nodes.Select(x => (x.Id, x.ParentId)), filter.CategoryId.Value).ToArray();
                query = query.Where(x => x.StoreCategoryId.HasValue && ids.Contains(x.StoreCategoryId.Value));
            }
            var count = await query.CountAsync(ct);
            var products = await query.OrderByDescending(x => x.CreatedAtUtc).ThenBy(x => x.Id)
                .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
            var stock = await Stock(products, ct);
            return new(products.Select(x => SellerProductDto.From(x, stock)).ToList(), count, filter.Page, filter.PageSize);
        }
    }

}
