using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products.Events
{
    public sealed record ProductVariantPriceChangedDomainEvent(
    Guid ProductId,
    Guid ProductVariantId,
    decimal PreviousAmount,
    decimal CurrentAmount,
    string CurrencyCode,
    DateTime OccurredAtUtc)
    : IDomainEvent;
}
