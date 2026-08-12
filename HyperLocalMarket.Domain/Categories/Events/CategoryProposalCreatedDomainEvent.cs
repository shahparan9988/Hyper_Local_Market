using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories.Events
{
    public sealed record CategoryProposalCreatedDomainEvent(
        Guid CategoryProposalId,
        Guid ProductId,
        Guid StoreId,
        string ProposedName,
        DateTime OccurredAtUtc) : IDomainEvent;
}
