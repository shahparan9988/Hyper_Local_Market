using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class ProductOptionValue : Entity
    {
        public const int ValueMaxLength = 100;

        private ProductOptionValue()
        {
        }

        internal ProductOptionValue(
            Guid productOptionId,
            string value,
            int displayOrder)
        {
            Guard.NotEmpty(productOptionId, nameof(productOptionId));

            if (displayOrder < 0)
            {
                throw new DomainException("Display order cannot be negative.");
            }

            ProductOptionId = productOptionId;
            Value = Guard.RequiredText(value, nameof(value), ValueMaxLength);
            DisplayOrder = displayOrder;
        }

        public Guid ProductOptionId { get; private set; }

        public string Value { get; private set; } = null!;


        public int DisplayOrder { get; private set; }
        public bool IsListed { get; private set; } = true;

        internal static ProductOptionValue CreateCatalog(Guid optionId, CatalogOptionValue input, int order) =>
            new(optionId, input.Value, order) { Id = input.Id };

        internal void ApplyCatalog(string value, int order)
        {
            Value = Guard.RequiredText(value, nameof(value), ValueMaxLength);
            DisplayOrder = order;
            IsListed = true;
        }

        internal void RetireCatalog() => IsListed = false;


    }

}
