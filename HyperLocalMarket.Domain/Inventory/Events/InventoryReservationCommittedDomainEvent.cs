using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Inventory.Events
{
    public sealed record InventoryReservationCommittedDomainEvent(
        Guid InventoryItemId,
        Guid ProductVariantId,
        Guid ReservationId,
        decimal Quantity,
        DateTime OccurredAtUtc) : IDomainEvent;

}
