using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Stores.Events
{
    public sealed record StoreCreatedDomainEvent(
        string Name,
        Guid StoreId,
        Guid UserId,
        DateTime OccurredAtUtc) : IDomainEvent;
}
