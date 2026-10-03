using HyperLocalMarket.Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed partial class ProductVariant
    {
        public bool IsPriceSet { get; private set; } = true;
        public bool IsListed { get; private set; } = true;
        public int CatalogOrder { get; private set; }

        internal static ProductVariant CreateCatalog(Guid productId, CatalogVariantValue value,
            string currency, ProductType type, int order, bool isDefault, DateTime now)
        {
            Guard.NotEmpty(value.Id, nameof(value.Id));
            var variant = new ProductVariant(productId, value.Name, value.Sku, null,
                Money.Create(value.Price ?? 0, currency),
                value.CompareAtPrice.HasValue ? Money.Create(value.CompareAtPrice.Value, currency) : null,
                type == ProductType.Service ? SalesUnit.Service : SalesUnit.Each,
                1, 1, false, null, null, new Dictionary<Guid, Guid>(), now);
            variant.Id = value.Id;
            variant.ApplyCatalog(value, currency, type, order, isDefault, now);
            return variant;
        }

        internal void ApplyCatalog(CatalogVariantValue value, string currency, ProductType type, int order, bool isDefault, DateTime now)
        {
            DisplayName = Guard.RequiredText(value.Name, nameof(value.Name), DisplayNameMaxLength);
            Sku = Guard.OptionalText(value.Sku, nameof(value.Sku), SkuMaxLength);
            Price = Money.Create(value.Price ?? 0, currency);
            CompareAtPrice = value.CompareAtPrice.HasValue ? Money.Create(value.CompareAtPrice.Value, currency) : null;
            ValidatePricing(Price, CompareAtPrice);
            SalesUnit = type == ProductType.Service ? SalesUnit.Service
                : SalesUnit == SalesUnit.Service ? SalesUnit.Each : SalesUnit;
            IsPriceSet = value.Price.HasValue;
            IsListed = true;
            CatalogOrder = order;
            IsDefault = isDefault && value.IsOffered;
            Status = value.IsOffered ? ProductVariantStatus.Active : ProductVariantStatus.Inactive;
            // Preserve existing selection rows. Only legacy/new variants start empty.
            if (_selections.Count == 0)
                foreach (var selection in value.Selections ?? [])
                    _selections.Add(new ProductVariantSelection(Id, selection.OptionId, selection.ValueId));
            UpdatedAtUtc = now;
        }

        internal void RetireCatalog(DateTime now)
        {
            IsListed = false;
            IsDefault = false;
            Status = ProductVariantStatus.Inactive;
            UpdatedAtUtc = now;
        }

        internal void ClearCatalogUniqueFlags(bool clearSku)
        {
            IsDefault = false;
            if (clearSku) Sku = null;
        }
    }


}
