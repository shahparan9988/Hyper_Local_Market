using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Stores
{
    public sealed class DeliveryOptionRepository : IDeliveryOptionRepository
    {
        private readonly AppDbContext _db;
        public DeliveryOptionRepository(AppDbContext db) => _db = db;

        public Task<List<StoreDeliveryOption>> ListAsync(Guid storeId, CancellationToken ct) =>
            _db.Set<StoreDeliveryOption>().AsNoTracking()
                .Where(x => x.StoreId == storeId)
                .OrderByDescending(x => x.IsActive).ThenBy(x => x.Name).ThenBy(x => x.Id)
                .ToListAsync(ct);

        public Task<StoreDeliveryOption?> GetTrackedAsync(Guid storeId, Guid optionId, CancellationToken ct) =>
            _db.Set<StoreDeliveryOption>().AsTracking()
                .SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == optionId, ct);

        public async Task AddAsync(StoreDeliveryOption option, CancellationToken ct)
        {
            await _db.Set<StoreDeliveryOption>().AddAsync(option, ct);
        }

        public Task<Product?> GetProductTrackedAsync(Guid storeId, Guid productId, CancellationToken ct) =>
            _db.Products.AsTracking().Include(x => x.DeliveryOptions)
                .SingleOrDefaultAsync(x => x.StoreId == storeId && x.Id == productId, ct);

        public async Task<PublicProductDeliveryDto?> GetPublicAsync(Guid productId, CancellationToken ct)
        {
            var info = await (
                from product in _db.Products.AsNoTracking()
                join store in _db.Stores.AsNoTracking() on product.StoreId equals store.Id
                where product.Id == productId && product.Status == ProductStatus.Active &&
                      store.Status == StoreStatus.Active
                select new
                {
                    product.Id,
                    product.StoreId,
                    IsPickupAvailable = store.IsPickupAvailable && product.IsPickupAvailable && product.Type == ProductType.Product,
                    IsDeliveryAvailable = store.IsDeliveryAvailable && product.Type == ProductType.Product,
                    store.FulfillmentNotes
                })
                .SingleOrDefaultAsync(ct);
            if (info is null) return null;

            var options = new List<StoreDeliveryOption>();
            if (info.IsDeliveryAvailable)
            {
                options = await (
                    from link in _db.Set<ProductDeliveryOption>().AsNoTracking()
                    join option in _db.Set<StoreDeliveryOption>().AsNoTracking()
                        on link.DeliveryOptionId equals option.Id
                    where link.ProductId == productId && link.StoreId == info.StoreId &&
                          option.StoreId == info.StoreId && option.IsActive
                    orderby option.Name, option.Id
                    select option).ToListAsync(ct);
            }

            return new PublicProductDeliveryDto(info.Id, info.IsPickupAvailable,
                info.IsDeliveryAvailable, info.FulfillmentNotes,
                options.Select(DeliveryOptionDto.From).ToList());
        }
    }

}
