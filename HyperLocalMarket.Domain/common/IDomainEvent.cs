using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOnUtc { get; }
    }
}
