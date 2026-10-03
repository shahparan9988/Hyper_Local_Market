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

        public bool IsListed { get; private set; } = true;

        public IReadOnlyCollection<ProductOptionValue> Values =>
            _values.AsReadOnly();


        internal static ProductOption CreateCatalog(Guid productId, CatalogOption input, int order)
        {
            var option = new ProductOption(productId, input.Name, order) { Id = input.Id };
            option.ApplyCatalog(input, order);
            return option;
        }

        internal void StageCatalogEdit()
        {
            IsListed = false;
            foreach (var value in _values) value.RetireCatalog();
        }

        internal void ApplyCatalog(CatalogOption input, int order)
        {
            Name = Guard.RequiredText(input.Name, nameof(input.Name), NameMaxLength);
            DisplayOrder = order;
            IsListed = true;
            // Keep omitted values for historical variants that still reference them.
            foreach (var value in _values) value.RetireCatalog();
            for (var index = 0; index < input.Values.Count; index++)
            {
                var incoming = input.Values[index];
                var value = _values.SingleOrDefault(x => x.Id == incoming.Id);
                if (value is null) _values.Add(ProductOptionValue.CreateCatalog(Id, incoming, index));
                else value.ApplyCatalog(incoming.Value, index);
            }
        }

        internal ProductOptionValue AddValue(
            string value,
            int displayOrder)
        {
            var normalizedValue = Guard.RequiredText(
                value,
                nameof(value),
                ProductOptionValue.ValueMaxLength);

            if (_values.Any(existing =>
                    existing.IsListed && string.Equals(
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
            _values.Any(value => value.Id == valueId && value.IsListed);

        internal string GetValueText(Guid valueId)
        {
            return _values.SingleOrDefault(value => value.Id == valueId && value.IsListed)?.Value
                   ?? throw new DomainException(
                       $"Option value '{valueId}' does not belong to option '{Name}'.");
        }

    }
}


