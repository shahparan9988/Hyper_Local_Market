using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class ProductVariant : Entity
    {
        public const int SkuMaxLength = 100;
        public const int BarcodeMaxLength = 100;
        public const int DisplayNameMaxLength = 500;

        private readonly List<ProductVariantSelection> _selections = [];

        private ProductVariant()
        {
        }

        internal ProductVariant(
            Guid productId,
            string displayName,
            string? sku,
            string? barcode,
            Money price,
            Money? compareAtPrice,
            SalesUnit salesUnit,
            decimal minimumOrderQuantity,
            decimal quantityIncrement,
            bool isDefault,
            Weight? shippingWeight,
            Dimensions? shippingDimensions,
            IReadOnlyDictionary<Guid, Guid> selectedOptionValues,
            DateTime utcNow)
        {
            Guard.NotEmpty(productId, nameof(productId));
            ArgumentNullException.ThrowIfNull(price);
            ArgumentNullException.ThrowIfNull(selectedOptionValues);
            Guard.Utc(utcNow, nameof(utcNow));

            ValidatePricing(price, compareAtPrice);
            ValidateSalesRules(salesUnit, minimumOrderQuantity, quantityIncrement);

            ProductId = productId;
            DisplayName = Guard.RequiredText(
                displayName,
                nameof(displayName),
                DisplayNameMaxLength);
            Sku = Guard.OptionalText(sku, nameof(sku), SkuMaxLength);
            Barcode = Guard.OptionalText(
                barcode,
                nameof(barcode),
                BarcodeMaxLength);
            Price = price;
            CompareAtPrice = compareAtPrice;
            SalesUnit = salesUnit;
            MinimumOrderQuantity = minimumOrderQuantity;
            QuantityIncrement = quantityIncrement;
            IsDefault = isDefault;
            ShippingWeight = shippingWeight;
            ShippingDimensions = shippingDimensions;
            Status = ProductVariantStatus.Active;
            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;

            /*
             * At this point, Id has already been generated
             * by the Entity base class.
             */
            foreach (var selectedValue in selectedOptionValues)
            {
                var selection = new ProductVariantSelection(
                    Id,
                    selectedValue.Key,
                    selectedValue.Value);

                _selections.Add(selection);
            }
        }

        public Guid ProductId { get; private set; }

        public string DisplayName { get; private set; } = null!;

        public string? Sku { get; private set; }

        public string? Barcode { get; private set; }

        public Money Price { get; private set; } = null!;

        public Money? CompareAtPrice { get; private set; }

        public SalesUnit SalesUnit { get; private set; }

        public decimal MinimumOrderQuantity { get; private set; }

        public decimal QuantityIncrement { get; private set; }

        public bool IsDefault { get; private set; }
        
        public Weight? ShippingWeight { get; private set; }

        public Dimensions? ShippingDimensions { get; private set; }

        public ProductVariantStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public IReadOnlyCollection<ProductVariantSelection> Selections =>
            _selections.AsReadOnly();

        public bool IsActive => Status == ProductVariantStatus.Active;

        public bool IsValidOrderQuantity(decimal quantity)
        {
            if (quantity < MinimumOrderQuantity)
            {
                return false;
            }

            var increments = (quantity - MinimumOrderQuantity) / QuantityIncrement;
            return increments == decimal.Truncate(increments);
        }

        internal void ChangePrice(
            Money price,
            Money? compareAtPrice,
            DateTime utcNow)
        {
            ArgumentNullException.ThrowIfNull(price);
            Guard.Utc(utcNow, nameof(utcNow));
            ValidatePricing(price, compareAtPrice);

            if (!Price.HasSameCurrency(price))
            {
                throw new DomainException(
                    "A variant's currency cannot be changed through a price update.");
            }

            Price = price;
            CompareAtPrice = compareAtPrice;
            UpdatedAtUtc = utcNow;
        }

        internal void UpdateIdentifiers(
            string? sku,
            string? barcode,
            DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            Sku = Guard.OptionalText(sku, nameof(sku), SkuMaxLength);
            Barcode = Guard.OptionalText(
                barcode,
                nameof(barcode),
                BarcodeMaxLength);
            UpdatedAtUtc = utcNow;
        }

        internal void UpdateSalesRules(
            SalesUnit salesUnit,
            decimal minimumOrderQuantity,
            decimal quantityIncrement,
            DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));
            ValidateSalesRules(
                salesUnit,
                minimumOrderQuantity,
                quantityIncrement);

            SalesUnit = salesUnit;
            MinimumOrderQuantity = minimumOrderQuantity;
            QuantityIncrement = quantityIncrement;
            UpdatedAtUtc = utcNow;
        }

        internal void UpdateShippingInformation(
            Weight? shippingWeight,
            Dimensions? shippingDimensions,
            DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            ShippingWeight = shippingWeight;
            ShippingDimensions = shippingDimensions;
            UpdatedAtUtc = utcNow;
        }

        internal void Activate(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));
            Status = ProductVariantStatus.Active;
            UpdatedAtUtc = utcNow;
        }

        internal void Deactivate(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            if (IsDefault)
            {
                throw new DomainException(
                    "The default variant cannot be deactivated. Select another default first.");
            }

            Status = ProductVariantStatus.Inactive;
            UpdatedAtUtc = utcNow;
        }

        internal void MakeDefault(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            if (!IsActive)
            {
                throw new DomainException(
                    "An inactive variant cannot become the default.");
            }

            IsDefault = true;
            UpdatedAtUtc = utcNow;
        }

        internal void RemoveDefault(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));
            IsDefault = false;
            UpdatedAtUtc = utcNow;
        }

        private static void ValidatePricing(
            Money price,
            Money? compareAtPrice)
        {
            if (compareAtPrice is null)
            {
                return;
            }

            if (!price.HasSameCurrency(compareAtPrice))
            {
                throw new DomainException(
                    "Price and compare-at price must use the same currency.");
            }

            if (compareAtPrice.Amount <= price.Amount)
            {
                throw new DomainException(
                    "Compare-at price must be greater than the selling price.");
            }
        }

        private static void ValidateSalesRules(
            SalesUnit salesUnit,
            decimal minimumOrderQuantity,
            decimal quantityIncrement)
        {
            if (!Enum.IsDefined(salesUnit))
            {
                throw new DomainException("Sales unit is invalid.");
            }

            if (minimumOrderQuantity <= 0)
            {
                throw new DomainException(
                    "Minimum order quantity must be greater than zero.");
            }

            if (quantityIncrement <= 0)
            {
                throw new DomainException(
                    "Quantity increment must be greater than zero.");
            }

            if (salesUnit is SalesUnit.Each
                or SalesUnit.Pack
                or SalesUnit.Box
                or SalesUnit.Dozen
                or SalesUnit.Service)
            {
                if (minimumOrderQuantity != decimal.Truncate(minimumOrderQuantity) ||
                    quantityIncrement != decimal.Truncate(quantityIncrement))
                {
                    throw new DomainException(
                        $"Sales unit '{salesUnit}' requires whole-number order quantities.");
                }
            }
        }
    }
}
