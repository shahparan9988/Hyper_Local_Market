using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories.Events
{
    public sealed record CategoryProposalResolvedDomainEvent(
        Guid CategoryProposalId,
        Guid ProductId,
        CategoryProposalStatus Status,
        Guid? ResolvedCategoryId,
        DateTime OccurredAtUtc) : IDomainEvent;
}
