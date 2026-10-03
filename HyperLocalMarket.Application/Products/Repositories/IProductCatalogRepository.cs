using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Repositories
{
    public interface ICatalogTransaction : IAsyncDisposable
    {
        Store Store { get; }
        Task CommitAsync(CancellationToken ct);
    }
    public sealed record CatalogReceipt(string Hash, string ResultJson);
    public interface IProductCatalogRepository
    {
        Task<ICatalogTransaction> BeginOwnerWriteAsync(Guid storeId, Guid userId, CancellationToken ct);
        Task<Product?> GetTrackedAsync(Guid storeId, Guid productId, CancellationToken ct);
        Task<Dictionary<Guid, InventoryItem>> GetInventoryAsync(IEnumerable<Guid> variantIds, CancellationToken ct);
        Task<bool> HasForeignOptionIdsAsync(Guid productId, IReadOnlyList<CatalogOption> options, CancellationToken ct);
        Task<bool> HasForeignVariantIdAsync(Guid productId, IEnumerable<Guid> ids, CancellationToken ct);
        Task<bool> NameExistsAsync(Guid storeId, string name, Guid? exceptId, CancellationToken ct);
        Task<List<StoreProductCategory>> CategoriesAsync(Guid storeId, CancellationToken ct);
        Task<Category?> MarketplaceCategoryAsync(Guid id, CancellationToken ct);
        Task<List<StoreDeliveryOption>> LockDeliveryOptionsAsync(Guid storeId, IReadOnlyList<Guid> ids, CancellationToken ct);
        Task<List<ProductImageAsset>> LockImagesAsync(Guid storeId, Guid userId, IReadOnlyList<Guid> ids, CancellationToken ct);
        Task<CatalogReceipt?> ReceiptAsync(Guid storeId, Guid userId, string operation, Guid key, CancellationToken ct);
        void AddReceipt(Guid storeId, Guid userId, string operation, Guid key, string hash, string json, DateTime now);
        void Add(Product product);
        void Add(StoreProductCategory category);
        void Add(InventoryItem inventory);
        void Add(InventoryAdjustment adjustment);
    }
}
