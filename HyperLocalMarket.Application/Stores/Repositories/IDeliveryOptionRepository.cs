using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Repositories
{
    public interface IDeliveryOptionRepository
    {
        Task<List<StoreDeliveryOption>> ListAsync(Guid storeId, CancellationToken ct);
        Task<StoreDeliveryOption?> GetTrackedAsync(Guid storeId, Guid optionId, CancellationToken ct);
        Task AddAsync(StoreDeliveryOption option, CancellationToken ct);
        Task<Product?> GetProductTrackedAsync(Guid storeId, Guid productId, CancellationToken ct);
        Task<PublicProductDeliveryDto?> GetPublicAsync(Guid productId, CancellationToken ct);
    }

    public sealed record PublicProductDeliveryDto(
        Guid ProductId, bool IsPickupAvailable, bool IsDeliveryAvailable,
        string? FulfillmentNotes, IReadOnlyList<DeliveryOptionDto> Options);

    public sealed record ProductDeliverySelectionDto(
        Guid ProductId, int Version, IReadOnlyList<Guid> DeliveryOptionIds,
        IReadOnlyList<DeliveryOptionDto> Options);

}
