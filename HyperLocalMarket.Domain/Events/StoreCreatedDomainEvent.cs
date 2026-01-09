using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Events
{
    public sealed class StoreCreatedDomainEvent : IDomainEvent
    {
        public Guid StoreId { get; }
        public string Name { get; }
        public DateTime OccurredOnUtc { get; }
        public StoreCreatedDomainEvent(Guid storeId, string name) 
        {
            StoreId = storeId;
            Name = name;
            OccurredOnUtc = DateTime.UtcNow;
        }
    }
}
