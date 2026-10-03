using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Dtos
{
    public sealed record ProductOptionValueDto(Guid Id, string Value);
    public sealed record ProductOptionDto(Guid Id, string Name, IReadOnlyList<ProductOptionValueDto> Values);
    public sealed record ProductVariantSelectionDto(Guid OptionId, Guid ValueId);
    public sealed record ProductInventoryDto(decimal OnHandQuantity, decimal ReservedQuantity, int? Version);
    public sealed record ProductVariantDto(Guid Id, string Name, string Sku, decimal? Price,
        decimal? CompareAtPrice, ProductInventoryDto Inventory,
        bool IsOffered, IReadOnlyList<ProductVariantSelectionDto> Selections);
    public sealed record ProductImageDto(Guid AssetId, string ImageUrl);
    public sealed record SellerProductDto(Guid Id, Guid StoreId, int Version, string Name,
        string Description, string Type, string Status, string CurrencyCode,
        Guid? StoreCategoryId, Guid? MarketplaceCategoryId, bool TrackInventory,
        string ManualAvailability, string VariantOptionName, IReadOnlyList<ProductVariantDto> Variants,
        IReadOnlyList<ProductImageDto> Images, bool IsPickupAvailable,
        IReadOnlyList<Guid> DeliveryOptionIds, DateTime UpdatedAtUtc,
        IReadOnlyList<ProductOptionDto> Options)
    {
        public static SellerProductDto From(Product p, IReadOnlyDictionary<Guid, InventoryItem> stock) => new(
            p.Id, p.StoreId, p.Version, p.Name, p.Description ?? "", p.Type.ToString(),
            p.Status == ProductStatus.Active ? "Published" : p.Status.ToString(), p.CurrencyCode,
            p.StoreCategoryId, p.CategoryId, p.TrackInventory, p.ManualAvailability.ToString(), p.VariantOptionName,
            p.Variants.Where(v => v.IsListed).OrderBy(v => v.CatalogOrder).ThenBy(v => v.Id).Select(v => new ProductVariantDto(
                v.Id, v.DisplayName, v.Sku ?? "", v.IsPriceSet ? v.Price.Amount : null, v.CompareAtPrice?.Amount,
                stock.TryGetValue(v.Id, out var i) ? new(i.OnHandQuantity, i.ReservedQuantity, i.Version) : new(0, 0, null),
                v.IsActive, v.Selections.Select(s => new ProductVariantSelectionDto(s.ProductOptionId, s.ProductOptionValueId)).ToList())).ToList(),
            p.Images.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Select(x => new ProductImageDto(x.AssetId ?? x.Id, x.Url)).ToList(),
            p.IsPickupAvailable, p.DeliveryOptions.Select(x => x.DeliveryOptionId).OrderBy(x => x).ToList(), p.UpdatedAtUtc,
            p.Options.Where(o => o.IsListed).OrderBy(o => o.DisplayOrder).ThenBy(o => o.Id)
                .Select(o => new ProductOptionDto(o.Id, o.Name,
                    o.Values.Where(v => v.IsListed).OrderBy(v => v.DisplayOrder).ThenBy(v => v.Id)
                        .Select(v => new ProductOptionValueDto(v.Id, v.Value)).ToList())).ToList());
    }
    public sealed record ProductPageDto(IReadOnlyList<SellerProductDto> Items, int TotalCount, int Page, int PageSize);
    public sealed record StoreCategoryDto(Guid Id, Guid StoreId, Guid? ParentId, string Name, int SortOrder, int Version)
    {
        public static StoreCategoryDto From(StoreProductCategory c) => new(c.Id, c.StoreId, c.ParentId, c.Name, c.SortOrder, c.Version);
    }
    public sealed record MarketplaceCategoryDto(Guid Id, Guid? ParentId, string Name, int SortOrder);
    public sealed record ProductEditorStoreDto(Guid Id, string Name, string Status, string Slug,
        string CountryCode, string? LogoUrl, string? CoverImageUrl, bool IsPickupAvailable, bool IsDeliveryAvailable);
    public sealed record ProductEditorDataDto(ProductEditorStoreDto Store, string CurrencyCode,
        IReadOnlyList<StoreCategoryDto> StoreCategories, IReadOnlyList<MarketplaceCategoryDto> MarketplaceCategories,
        IReadOnlyList<DeliveryOptionDto> DeliveryOptions);
}
