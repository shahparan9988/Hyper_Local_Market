using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products.Events
{
    public sealed record ProductCreatedDomainEvent(
        Guid ProductId,
        Guid StoreId,
        DateTime OccurredAtUtc) : IDomainEvent;
}
