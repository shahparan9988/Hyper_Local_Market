using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class ProductVariantSelection : Entity
    {
        private ProductVariantSelection()
        {
        }

        internal ProductVariantSelection(
            Guid productVariantId,
            Guid productOptionId,
            Guid productOptionValueId)
        {
            Guard.NotEmpty(productVariantId, nameof(productVariantId));
            Guard.NotEmpty(productOptionId, nameof(productOptionId));
            Guard.NotEmpty(productOptionValueId, nameof(productOptionValueId));

            ProductVariantId = productVariantId;
            ProductOptionId = productOptionId;
            ProductOptionValueId = productOptionValueId;
        }

        public Guid ProductVariantId { get; private set; }

        public Guid ProductOptionId { get; private set; }

        public Guid ProductOptionValueId { get; private set; }

    }
}
