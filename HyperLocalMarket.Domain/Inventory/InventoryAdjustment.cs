using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Inventory
{
    public sealed class InventoryAdjustment : Entity
    {
        private InventoryAdjustment() { }
        public InventoryAdjustment(
            Guid storeId,
            Guid productId,
            Guid variantId,
            Guid userId,
            decimal previous,
            decimal current,
            string reason,
            DateTime now)
        {
            StoreId = storeId;
            ProductId = productId;
            ProductVariantId = variantId;
            UserId = userId;
            PreviousQuantity = previous;
            NewQuantity = current;
            Reason = Guard.RequiredText(
                reason,
                nameof(reason),
                50);
            Guard.Utc(now, nameof(now));
            CreatedAtUtc = now;
        }
        public Guid StoreId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal PreviousQuantity { get; private set; }
        public decimal NewQuantity { get; private set; }
        public string Reason { get; private set; } = "";
        public DateTime CreatedAtUtc { get; private set; }
    }

}
