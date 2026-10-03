using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class StoreProductCategory : AggregateRoot
    {
        private StoreProductCategory() { }
        public StoreProductCategory(
            Guid storeId,
            Guid? parentId,
            string name,
            DateTime now)
        {
            Guard.NotEmpty(
                storeId,
                nameof(storeId));

            Guard.Utc(now, nameof(now));

            if (parentId == Guid.Empty
                || parentId == Id)
            {
                throw new DomainException("Invalid parent category.");
            }
                
            StoreId = storeId;
            ParentId = parentId;
            Name = Guard.RequiredText(name, nameof(name), 100);
            NormalizedName = Name.ToUpperInvariant();
            CreatedAtUtc = now;
        }
        public Guid StoreId { get; private set; }
        public Guid? ParentId { get; private set; }
        public string Name { get; private set; } = "";
        public string NormalizedName { get; private set; } = "";
        public int SortOrder { get; private set; }
        public int Version { get; private set; } = 1;
        public DateTime CreatedAtUtc { get; private set; }
    }

}
