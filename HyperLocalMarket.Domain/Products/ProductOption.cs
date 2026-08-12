using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class ProductOption : Entity
    {
        public const int NameMaxLength = 50;

        private readonly List<ProductOptionValue> _values = [];

        private ProductOption()
        {
        }

        internal ProductOption(
            Guid productId,
            string name,
            int displayOrder)
        {
            Guard.NotEmpty(productId, nameof(productId));

            if (displayOrder < 0)
            {
                throw new DomainException("Display order cannot be negative.");
            }

            ProductId = productId;
            Name = Guard.RequiredText(name, nameof(name), NameMaxLength);
            DisplayOrder = displayOrder;
        }

        public Guid ProductId { get; private set; }

        public string Name { get; private set; } = null!;

        public int DisplayOrder { get; private set; }

        public IReadOnlyCollection<ProductOptionValue> Values =>
            _values.AsReadOnly();

        internal ProductOptionValue AddValue(
            string value,
            int displayOrder)
        {
            var normalizedValue = Guard.RequiredText(
                value,
                nameof(value),
                ProductOptionValue.ValueMaxLength);

            if (_values.Any(existing =>
                    string.Equals(
                        existing.Value,
                        normalizedValue,
                        StringComparison.OrdinalIgnoreCase)))
            {
                throw new DomainException(
                    $"Option '{Name}' already contains value '{normalizedValue}'.");
            }

            var optionValue = new ProductOptionValue(
                Id,
                normalizedValue,
                displayOrder);

            _values.Add(optionValue);
            return optionValue;
        }

        internal bool ContainsValue(Guid valueId) =>
            _values.Any(value => value.Id == valueId);

        internal string GetValueText(Guid valueId)
        {
            return _values.SingleOrDefault(value => value.Id == valueId)?.Value
                   ?? throw new DomainException(
                       $"Option value '{valueId}' does not belong to option '{Name}'.");
        }

    }
}
