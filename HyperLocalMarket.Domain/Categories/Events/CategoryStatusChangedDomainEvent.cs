using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories.Events
{
    public sealed record CategoryStatusChangedDomainEvent(
        Guid CategoryId,
        CategoryStatus PreviousStatus,
        CategoryStatus CurrentStatus,
        DateTime OccurredAtUtc) : IDomainEvent;
}
