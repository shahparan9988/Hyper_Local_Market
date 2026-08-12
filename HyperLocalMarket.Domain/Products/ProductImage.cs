using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class ProductImage : Entity
    {
        public const int UrlMaxLength = 2_000;
        public const int AltTextMaxLength = 300;

        private ProductImage()
        {
        }

        internal ProductImage(
            Guid productId,
            Guid? productVariantId,
            string url,
            string? altText,
            int displayOrder,
            bool isPrimary)
        {
            Guard.NotEmpty(productId, nameof(productId));

            if (productVariantId == Guid.Empty)
            {
                throw new DomainException(
                    "Product variant identifier cannot be empty.");
            }

            if (displayOrder < 0)
            {
                throw new DomainException("Display order cannot be negative.");
            }

            ProductId = productId;
            ProductVariantId = productVariantId;
            Url = Guard.RequiredText(url, nameof(url), UrlMaxLength);
            AltText = Guard.OptionalText(
                altText,
                nameof(altText),
                AltTextMaxLength);
            DisplayOrder = displayOrder;
            IsPrimary = isPrimary;
        }

        public Guid ProductId { get; private set; }

        public Guid? ProductVariantId { get; private set; }

        public string Url { get; private set; } = null!;

        public string? AltText { get; private set; }

        public int DisplayOrder { get; private set; }

        public bool IsPrimary { get; private set; }

        internal void MakePrimary() => IsPrimary = true;

        internal void RemovePrimary() => IsPrimary = false;

    }
}
