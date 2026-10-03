using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class ProductDeliveryOption
    {
        private ProductDeliveryOption() { }

        internal ProductDeliveryOption(Guid storeId, Guid productId, Guid deliveryOptionId)
        {
            StoreId = storeId;
            ProductId = productId;
            DeliveryOptionId = deliveryOptionId;
        }

        public Guid StoreId { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid DeliveryOptionId { get; private set; }
    }

}
