using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Inventory.Events
{
    public sealed record InventoryAdjustedDomainEvent(
    Guid InventoryItemId,
    Guid ProductVariantId,
    decimal PreviousOnHandQuantity,
    decimal CurrentOnHandQuantity,
    string Reason,
    DateTime OccurredAtUtc) : IDomainEvent;
}
