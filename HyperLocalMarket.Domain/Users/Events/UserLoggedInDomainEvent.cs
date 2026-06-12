using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Users.Events
{
    public sealed record UserLoggedInDomainEvent(
        Guid UserId,
        Guid SessionId,
        DateTime OccurredAtUtc
        ) : IDomainEvent;
 }
