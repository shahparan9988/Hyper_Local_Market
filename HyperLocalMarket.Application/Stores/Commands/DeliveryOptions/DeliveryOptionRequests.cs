using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.DeliveryOptions
{
    public sealed record GetDeliveryOptionsQuery(Guid StoreId, Guid UserId)
        : IRequest<IReadOnlyList<DeliveryOptionDto>>;
    public sealed record CreateDeliveryOptionCommand(Guid StoreId, Guid UserId, DeliveryOptionInput Input)
        : IRequest<DeliveryOptionDto>;
    public sealed record UpdateDeliveryOptionCommand(
        Guid StoreId, Guid UserId, Guid OptionId, int ExpectedVersion, DeliveryOptionInput Input)
        : IRequest<DeliveryOptionDto>;
    public sealed record SetDeliveryOptionStatusCommand(
        Guid StoreId, Guid UserId, Guid OptionId, int ExpectedVersion, bool IsActive)
        : IRequest<DeliveryOptionDto>;
    public sealed record SaveFulfillmentCommand(
        Guid StoreId, Guid UserId, bool IsPickupAvailable,
        bool IsDeliveryAvailable, string? FulfillmentNotes) : IRequest<FulfillmentDto>;
    public sealed record GetProductDeliverySelectionQuery(Guid StoreId, Guid UserId, Guid ProductId)
        : IRequest<ProductDeliverySelectionDto>;
    public sealed record SaveProductDeliverySelectionCommand(
        Guid StoreId, Guid UserId, Guid ProductId, int ExpectedVersion,
        IReadOnlyList<Guid>? DeliveryOptionIds) : IRequest<ProductDeliverySelectionDto>;
    public sealed record GetPublicProductDeliveryQuery(Guid ProductId)
        : IRequest<PublicProductDeliveryDto?>;

}
