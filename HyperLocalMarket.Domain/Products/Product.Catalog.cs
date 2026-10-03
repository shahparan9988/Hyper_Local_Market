using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Products.Events;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed partial class Product
    {
        public int Version { get; private set; }
        public ProductType Type { get; private set; } = ProductType.Product;
        public ProductAvailability ManualAvailability { get; private set; } = ProductAvailability.Available;
        public Guid? StoreCategoryId { get; private set; }
        public string CurrencyCode { get; private set; } = "BDT";
        public bool TrackInventory { get; private set; }
        public bool IsPickupAvailable { get; private set; }
        public string VariantOptionName { get; private set; } = "";

        public void Touch(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));
            Version = checked(Version + 1);
            UpdatedAtUtc = utcNow;
        }

        // The handler persists these intermediate values inside the same transaction.
        // This prevents transient violations of the unique default/SKU/primary-image indexes.
        public void PrepareCatalogReplacement(IReadOnlySet<Guid> editedVariantIds, DateTime now)
        {
            EnsureNotArchived();
            foreach (var variant in _variants)
                variant.ClearCatalogUniqueFlags(editedVariantIds.Contains(variant.Id));
            foreach (var image in _images) image.RemovePrimary();
            foreach (var option in _options) option.StageCatalogEdit();
            Touch(now);
        }

        public void ApplyCatalog(string name, string? description, ProductType type,
            ProductAvailability availability, string currency, Guid? storeCategoryId,
            Guid? marketplaceCategoryId, bool trackInventory, bool pickup,
            string? optionName, IReadOnlyList<CatalogVariantValue> variants,
            IReadOnlyList<CatalogImageValue> images, DateTime now,
            IReadOnlyList<CatalogOption>? options = null)
        {
            EnsureNotArchived();
            options ??= [];
            ValidateCatalogOptions(options, variants);
            if (!Enum.IsDefined(type) || !Enum.IsDefined(availability)) throw new DomainException("Invalid listing type or availability.");
            if (currency is not ("BDT" or "AUD")) throw new DomainException("Choose BDT or AUD.");
            if (variants.Count == 0 || variants.Select(x => x.Id).Distinct().Count() != variants.Count)
                throw new DomainException("Supply at least one variant with distinct IDs.");
            var skus = variants.Where(x => !string.IsNullOrWhiteSpace(x.Sku)).Select(x => x.Sku!.Trim().ToUpperInvariant()).ToList();
            if (skus.Distinct().Count() != skus.Count) throw new DomainException("Variant SKUs must be unique.");
            foreach (var value in variants)
            {
                ValidateCatalogPrice(value.Price);
                ValidateCatalogPrice(value.CompareAtPrice);
                if (value.CompareAtPrice.HasValue && (!value.Price.HasValue || value.CompareAtPrice <= value.Price))
                    throw new DomainException("Original price must exceed the selling price.");
            }
            if (images.Count > 8 || images.Select(x => x.AssetId).Distinct().Count() != images.Count)
                throw new DomainException("Select at most eight distinct images.");
            if (type == ProductType.Service && (trackInventory || pickup)) throw new DomainException("Services do not use physical inventory or pickup.");
            Name = Guard.RequiredText(name, nameof(name), 150);
            Description = Guard.OptionalText(description, nameof(description), DescriptionMaxLength);
            Type = type; ManualAvailability = availability; CurrencyCode = currency;
            StoreCategoryId = storeCategoryId; TrackInventory = trackInventory; IsPickupAvailable = pickup;
            // Kept for existing list clients; Options is the authoritative model.
            VariantOptionName = options.Count == 1 ? options[0].Name.Trim() : options.Count > 1 ? "Options" : "";
            ApplyCatalogOptions(options);
            if (marketplaceCategoryId != CategoryId)
            {
                if (marketplaceCategoryId.HasValue) SelectCategory(marketplaceCategoryId.Value, now);
                else ClearCategory(now);
            }
            var requestedIds = variants.Select(x => x.Id).ToHashSet();
            foreach (var old in _variants.Where(x => !requestedIds.Contains(x.Id))) old.RetireCatalog(now);
            var defaultId = variants.FirstOrDefault(x => x.IsOffered)?.Id;
            for (var index = 0; index < variants.Count; index++)
            {
                var input = variants[index];
                var value = input with { Name = ProductOptionSet.Name(options, input.Selections ?? []) };
                var current = _variants.SingleOrDefault(x => x.Id == value.Id);
                if (current is null)
                {
                    current = ProductVariant.CreateCatalog(Id, value, currency, type, index, value.Id == defaultId, now);
                    _variants.Add(current);
                    AddDomainEvent(new ProductVariantAddedDomainEvent(Id, current.Id, now));
                }
                else
                {
                    var oldAmount = current.Price.Amount;
                    var hadPrice = current.IsPriceSet;
                    current.ApplyCatalog(value, currency, type, index, value.Id == defaultId, now);
                    if (hadPrice && value.Price.HasValue && oldAmount != value.Price.Value)
                        AddDomainEvent(new ProductVariantPriceChangedDomainEvent(Id, current.Id, oldAmount, value.Price.Value, currency, now));
                }
            }
            var imageIds = images.Select(x => x.AssetId).ToHashSet();
            _images.RemoveAll(x => !imageIds.Contains(x.AssetId ?? x.Id));
            for (var index = 0; index < images.Count; index++)
            {
                var value = images[index];
                var image = _images.SingleOrDefault(x => (x.AssetId ?? x.Id) == value.AssetId);
                if (image is null)
                {
                    if (value.IsLegacy) throw new DomainException("The old image no longer belongs to this product.");
                    image = new ProductImage(Id, null, value.Url, null, index, false);
                    image.AttachAsset(value.AssetId);
                    _images.Add(image);
                }
                image.SetCatalogOrder(index);
            }
            Touch(now);
        }

        public void SetManualAvailability(ProductAvailability availability, DateTime now)
        {
            EnsureNotArchived();
            if (TrackInventory || !Enum.IsDefined(availability)) throw new DomainException("Manual availability is only for untracked listings.");
            ManualAvailability = availability;
            Touch(now);
        }

        public void RestoreCatalogDraft(DateTime now)
        {
            Status = ProductStatus.Draft;
            ArchivedAtUtc = null;
            Touch(now);
        }

        private static void ValidateCatalogPrice(decimal? value)
        {
            if (value.HasValue && (value < 0 || value > 999999999.99m || decimal.Round(value.Value, 2) != value.Value))
                throw new DomainException("A price must be non-negative, up to 999999999.99, with at most two decimals.");
        }
    }


}
