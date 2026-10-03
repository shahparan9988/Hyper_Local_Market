using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Infrastructure.Persistence.Idempotency;
using HyperLocalMarket.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Products
{
    public sealed class ProductCatalogRepository(AppDbContext db) : IProductCatalogRepository
    {
        public async Task<ICatalogTransaction> BeginOwnerWriteAsync(Guid storeId, Guid userId, CancellationToken ct)
        {
            var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            try
            {
                // This row lock serializes catalog writes/idempotent creates for one store.
                // It also protects store fulfilment flags from changes until this commit.
                await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM \"Stores\" WHERE \"Id\" = {storeId} AND \"UserId\" = {userId} FOR UPDATE", ct);
                var store = await db.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.Id == storeId && x.UserId == userId, ct)
                    ?? throw new NotFoundException("Store was not found.");
                return new CatalogTransaction(transaction, store);
            }
            catch { await transaction.DisposeAsync(); throw; }
        }
        private sealed class CatalogTransaction(IDbContextTransaction transaction, Store store) : ICatalogTransaction
        {
            public Store Store => store;
            public Task CommitAsync(CancellationToken ct) => transaction.CommitAsync(ct);
            public ValueTask DisposeAsync() => transaction.DisposeAsync();
        }
        public Task<Product?> GetTrackedAsync(Guid storeId, Guid productId, CancellationToken ct) => db.Products.AsTracking()
            .Include(x => x.Options).ThenInclude(x => x.Values)
            .Include(x => x.Variants).ThenInclude(x => x.Selections)
            .Include(x => x.Images).Include(x => x.DeliveryOptions).AsSplitQuery()
            .SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == productId, ct);
        public Task<Dictionary<Guid, InventoryItem>> GetInventoryAsync(IEnumerable<Guid> variantIds, CancellationToken ct)
        {
            var ids = variantIds.Distinct().ToArray();
            return db.Set<InventoryItem>().AsTracking().Where(x => ids.Contains(x.ProductVariantId)).ToDictionaryAsync(x => x.ProductVariantId, ct);
        }

        public async Task<bool> HasForeignOptionIdsAsync(Guid productId, IReadOnlyList<CatalogOption> options, CancellationToken ct)
        {
            var optionIds = options.Select(x => x.Id).ToArray();
            var valueIds = options.SelectMany(x => x.Values).Select(x => x.Id).ToArray();
            if (await db.Set<ProductOption>().AnyAsync(x => optionIds.Contains(x.Id) && x.ProductId != productId, ct))
                return true;
            var expectedParents = options.SelectMany(o => o.Values.Select(v => (v.Id, OptionId: o.Id)))
                .ToDictionary(x => x.Id, x => x.OptionId);
            var existingValues = await db.Set<ProductOptionValue>().AsNoTracking()
                .Where(x => valueIds.Contains(x.Id)).Select(x => new { x.Id, x.ProductOptionId }).ToListAsync(ct);
            return existingValues.Any(x => expectedParents[x.Id] != x.ProductOptionId);
        }

        public Task<bool> HasForeignVariantIdAsync(Guid productId, IEnumerable<Guid> ids, CancellationToken ct)
        {
            var values = ids.ToArray();
            return db.Set<ProductVariant>().AnyAsync(x => values.Contains(x.Id) && x.ProductId != productId, ct);
        }
        public Task<bool> NameExistsAsync(Guid storeId, string name, Guid? exceptId, CancellationToken ct)
        {
            var normalized = name.ToLowerInvariant();
            return db.Products.AnyAsync(x => x.StoreId == storeId && x.Id != exceptId && x.Name.ToLower() == normalized, ct);
        }
        public Task<List<StoreProductCategory>> CategoriesAsync(Guid storeId, CancellationToken ct) =>
            db.Set<StoreProductCategory>().AsNoTracking().Where(x => x.StoreId == storeId).ToListAsync(ct);
        public async Task<Category?> MarketplaceCategoryAsync(Guid id, CancellationToken ct)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM \"Categories\" WHERE \"Id\" = {id} FOR SHARE", ct);
            return await db.Categories.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        }
        public Task<List<StoreDeliveryOption>> LockDeliveryOptionsAsync(Guid storeId, IReadOnlyList<Guid> ids, CancellationToken ct)
        {
            if (ids.Count == 0) return Task.FromResult(new List<StoreDeliveryOption>());
            var values = ids.ToArray();
            return db.Set<StoreDeliveryOption>().FromSqlInterpolated($"SELECT * FROM \"StoreDeliveryOptions\" WHERE \"StoreId\" = {storeId} AND \"Id\" = ANY({values}) ORDER BY \"Id\" FOR SHARE").AsNoTracking().ToListAsync(ct);
        }
        public Task<List<ProductImageAsset>> LockImagesAsync(Guid storeId, Guid userId, IReadOnlyList<Guid> ids, CancellationToken ct)
        {
            if (ids.Count == 0) return Task.FromResult(new List<ProductImageAsset>());
            var values = ids.ToArray();
            return db.Set<ProductImageAsset>().FromSqlInterpolated($"SELECT * FROM \"ProductImageAssets\" WHERE \"StoreId\" = {storeId} AND \"UserId\" = {userId} AND \"Id\" = ANY({values}) ORDER BY \"Id\" FOR SHARE").AsNoTracking().ToListAsync(ct);
        }
        public async Task<CatalogReceipt?> ReceiptAsync(Guid storeId, Guid userId, string operation, Guid key, CancellationToken ct)
        {
            var value = await db.Set<CatalogRequestReceipt>().AsNoTracking().SingleOrDefaultAsync(x => x.StoreId == storeId && x.UserId == userId && x.Operation == operation && x.RequestKey == key, ct);
            return value is null ? null : new(value.Hash, value.ResultJson);
        }
        public void AddReceipt(Guid storeId, Guid userId, string operation, Guid key, string hash, string json, DateTime now) =>
            db.Set<CatalogRequestReceipt>().Add(new() { StoreId = storeId, UserId = userId, Operation = operation, RequestKey = key, Hash = hash, ResultJson = json, CreatedAtUtc = now });
        public void Add(Product product) => db.Products.Add(product);
        public void Add(StoreProductCategory category) => db.Set<StoreProductCategory>().Add(category);
        public void Add(InventoryItem inventory) => db.Set<InventoryItem>().Add(inventory);
        public void Add(InventoryAdjustment adjustment) => db.Set<InventoryAdjustment>().Add(adjustment);
    }


}
