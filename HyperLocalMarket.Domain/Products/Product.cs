using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Products.Events;
using HyperLocalMarket.Shared.Exceptions;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guard = HyperLocalMarket.Domain.common.Guard;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class Product : AggregateRoot
    {
        public const int NameMaxLength = 200;
        public const int SlugMaxLength = 300;
        public const int DescriptionMaxLength = 5_000;
        public const int BrandMaxLength = 200;
        public const int MaximumOptionCount = 10;
        public const int MaximumImageCount = 20;

        private readonly List<ProductOption> _options = [];
        private readonly List<ProductVariant> _variants = [];
        private readonly List<ProductImage> _images = [];

        private Product()
        {
        }

        private Product(
            Guid storeId,
            string name,
            string slug,
            string? description,
            string? brand,
            DateTime utcNow)
        {
            Guard.NotEmpty(storeId, nameof(storeId));
            Guard.Utc(utcNow, nameof(utcNow));

            StoreId = storeId;
            Name = Guard.RequiredText(name, nameof(name), NameMaxLength);
            Slug = Guard.RequiredText(slug, nameof(slug), SlugMaxLength);
            Description = Guard.OptionalText(
                description,
                nameof(description),
                DescriptionMaxLength);
            Brand = Guard.OptionalText(brand, nameof(brand), BrandMaxLength);

            //CategoryId = null;
            //CategoryProposalId = null;
            CategoryAssignmentStatus = CategoryAssignmentStatus.Unassigned;
            Status = ProductStatus.Draft;
            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public Guid StoreId { get; private set; }

        public Guid? CategoryId { get; private set; }

        public Guid? CategoryProposalId { get; private set; }

        public CategoryAssignmentStatus CategoryAssignmentStatus
        {
            get;
            private set;
        }

        public string Name { get; private set; } = null!;

        public string Slug { get; private set; } = null!;

        public string? Description { get; private set; }

        public string? Brand { get; private set; }

        public ProductStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public DateTime? PublishedAtUtc { get; private set; }

        public DateTime? ArchivedAtUtc { get; private set; }

        public IReadOnlyCollection<ProductOption> Options =>
            _options.AsReadOnly();

        public IReadOnlyCollection<ProductVariant> Variants =>
            _variants.AsReadOnly();

        public IReadOnlyCollection<ProductImage> Images =>
            _images.AsReadOnly();

        public static Product Create(
            Guid storeId,
            string name,
            string slug,
            string? description,
            string? brand,
            DateTime utcNow)
        {
            var product = new Product(
                storeId,
                name,
                slug,
                description,
                brand,
                utcNow);

            product.AddDomainEvent(
                new ProductCreatedDomainEvent(
                    product.Id,
                    storeId,
                    utcNow));

            return product;
        }

        public void UpdateDetails(
            string name,
            string? description,
            string? brand,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            Name = Guard.RequiredText(name, nameof(name), NameMaxLength);
            Description = Guard.OptionalText(
                description,
                nameof(description),
                DescriptionMaxLength);
            Brand = Guard.OptionalText(brand, nameof(brand), BrandMaxLength);
            UpdatedAtUtc = utcNow;
        }

        public void ChangeCategory(Guid categoryId, DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.NotEmpty(categoryId, nameof(categoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            CategoryId = categoryId;
            UpdatedAtUtc = utcNow;
        }

        public ProductOption AddOption(
            string name,
            int displayOrder,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (_variants.Count > 0)
            {
                throw new DomainException(
                    "New options cannot be added after variants exist.");
            }

            if (_options.Count >= MaximumOptionCount)
            {
                throw new DomainException(
                    $"A product cannot contain more than {MaximumOptionCount} options.");
            }

            var normalizedName = Guard.RequiredText(
                name,
                nameof(name),
                ProductOption.NameMaxLength);

            if (_options.Any(option =>
                    string.Equals(
                        option.Name,
                        normalizedName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                throw new DomainException(
                    $"Product already contains option '{normalizedName}'.");
            }

            var option = new ProductOption(
                Id,
                normalizedName,
                displayOrder);

            _options.Add(option);
            UpdatedAtUtc = utcNow;
            return option;
        }

        public void SelectCategory(Guid categoryId, DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.NotEmpty(categoryId, nameof(categoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            // Avoid creating duplicate events for an identical request.
            if (CategoryId == categoryId &&
                CategoryProposalId is null &&
                CategoryAssignmentStatus ==
                    CategoryAssignmentStatus.PendingReview)
            {
                return;
            }

            CategoryId = categoryId;

            // The seller is selecting an existing category,
            // so a previous new-category proposal is no longer relevant.
            CategoryProposalId = null;

            CategoryAssignmentStatus =
                CategoryAssignmentStatus.PendingReview;

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductCategorySelectedDomainEvent(
                    Id,
                    categoryId,
                    CategoryAssignmentStatus,
                    utcNow));
        }

        public void AssignCategory(
            Guid categoryId,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.NotEmpty(categoryId, nameof(categoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            CategoryId = categoryId;
            UpdatedAtUtc = utcNow;
        }

        public void ConfirmCategory(Guid categoryId, DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.NotEmpty(categoryId, nameof(categoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            if (CategoryId == categoryId &&
                CategoryProposalId is null &&
                CategoryAssignmentStatus ==
                    CategoryAssignmentStatus.Confirmed)
            {
                return;
            }

            CategoryId = categoryId;

            // If this confirmation came from a proposal,
            // the proposal is now resolved.
            CategoryProposalId = null;

            CategoryAssignmentStatus =
                CategoryAssignmentStatus.Confirmed;

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductCategorySelectedDomainEvent(
                    Id,
                    categoryId,
                    CategoryAssignmentStatus,
                    utcNow));
        }


        public void LinkCategoryProposal(Guid categoryProposalId, DateTime utcNow)
        {
            EnsureNotArchived();

            Guard.NotEmpty(
                categoryProposalId,
                nameof(categoryProposalId));

            Guard.Utc(utcNow, nameof(utcNow));

            if (CategoryProposalId == categoryProposalId &&
                CategoryId is null &&
                CategoryAssignmentStatus ==
                    CategoryAssignmentStatus.PendingReview)
            {
                return;
            }

            // A proposed name is not yet an approved Category.
            CategoryId = null;

            CategoryProposalId = categoryProposalId;

            CategoryAssignmentStatus =
                CategoryAssignmentStatus.PendingReview;

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductCategoryProposalLinkedDomainEvent(
                    Id,
                    categoryProposalId,
                    utcNow));
        }


        public void MarkCategoryNeedsCorrection(DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (CategoryId is null &&
                CategoryProposalId is null)
            {
                throw new DomainException(
                    "The Product does not have a category selection or proposal to correct.");
            }

            if (CategoryAssignmentStatus ==
                CategoryAssignmentStatus.NeedsCorrection)
            {
                return;
            }

            CategoryAssignmentStatus =
                CategoryAssignmentStatus.NeedsCorrection;

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductCategoryNeedsCorrectionDomainEvent(
                    Id,
                    CategoryId,
                    CategoryProposalId,
                    utcNow));
        }

        public void ClearCategory(DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (CategoryId is null &&
                CategoryProposalId is null &&
                CategoryAssignmentStatus ==
                    CategoryAssignmentStatus.Unassigned)
            {
                return;
            }

            CategoryId = null;
            CategoryProposalId = null;

            CategoryAssignmentStatus =
                CategoryAssignmentStatus.Unassigned;

            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductCategoryClearedDomainEvent(
                    Id,
                    utcNow));
        }

        public ProductOptionValue AddOptionValue(
            Guid optionId,
            string value,
            int displayOrder,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            var option = GetOption(optionId);
            var optionValue = option.AddValue(
                value,
                displayOrder);

            UpdatedAtUtc = utcNow;
            return optionValue;
        }

        public ProductVariant AddVariant(
            string? sku,
            string? barcode,
            Money price,
            Money? compareAtPrice,
            SalesUnit salesUnit,
            decimal minimumOrderQuantity,
            decimal quantityIncrement,
            IReadOnlyDictionary<Guid, Guid> selectedOptionValues,
            bool makeDefault,
            Weight? shippingWeight,
            Dimensions? shippingDimensions,
            DateTime utcNow)
        {
            EnsureNotArchived();
            ArgumentNullException.ThrowIfNull(price);
            ArgumentNullException.ThrowIfNull(selectedOptionValues);
            Guard.Utc(utcNow, nameof(utcNow));

            ValidateSkuIsUnique(sku);
            ValidateBarcodeIsUnique(barcode);
            ValidateCurrency(price);

            if (_options.Count == 0 && _variants.Count > 0)
            {
                throw new DomainException(
                    "A product without options can contain only one variant.");
            }

            //var selections = CreateSelections(
            //    Guid.NewGuid(),
            //    selectedOptionValues,
            //    out var displayName,
            //    out var signature);



            // Validate the selected option IDs and calculate display information.
            var (displayName, signature) =
                ValidateAndDescribeSelections(selectedOptionValues);

            var duplicateExists = _variants.Any(variant =>
                BuildVariantSignature(
                    variant.Selections.Select(selection =>
                        (
                            selection.ProductOptionId,
                            selection.ProductOptionValueId
                        ))) == signature);

            //if (duplicateExists)
            //{
            //    throw new DomainException(
            //        "A variant with the same option combination already exists.");
            //}

            //if (_variants.Any(variant =>
            //        BuildVariantSignature(variant.Selections) == signature))
            //{
            //    throw new DomainException(
            //        "A variant with the same option combination already exists.");
            //}

            //var variantId = selections.Count == 0
            //    ? Guid.NewGuid()
            //    : selections[0].ProductVariantId;

            var shouldBeDefault = _variants.Count == 0 || makeDefault;

            if (shouldBeDefault)
            {
                RemoveCurrentDefault(utcNow);
            }

            var variant = new ProductVariant(
                Id,
                displayName,
                sku,
                barcode,
                price,
                compareAtPrice,
                salesUnit,
                minimumOrderQuantity,
                quantityIncrement,
                shouldBeDefault,
                shippingWeight,
                shippingDimensions,
                selectedOptionValues,
                utcNow);

            _variants.Add(variant);
            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductVariantAddedDomainEvent(
                    Id,
                    variant.Id,
                    utcNow));

            return variant;
        }

        public void ChangeVariantPrice(
            Guid variantId,
            Money price,
            Money? compareAtPrice,
            DateTime utcNow)
        {
            EnsureNotArchived();
            ArgumentNullException.ThrowIfNull(price);
            Guard.Utc(utcNow, nameof(utcNow));
            ValidateCurrency(price, variantId);

            var variant = GetVariant(variantId);
            var previousAmount = variant.Price.Amount;

            variant.ChangePrice(price, compareAtPrice, utcNow);
            UpdatedAtUtc = utcNow;

            if (previousAmount != price.Amount)
            {
                AddDomainEvent(
                    new ProductVariantPriceChangedDomainEvent(
                        Id,
                        variantId,
                        previousAmount,
                        price.Amount,
                        price.CurrencyCode,
                        utcNow));
            }
        }

        public void UpdateVariantIdentifiers(
            Guid variantId,
            string? sku,
            string? barcode,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            var variant = GetVariant(variantId);
            ValidateSkuIsUnique(sku, variantId);
            ValidateBarcodeIsUnique(barcode, variantId);

            variant.UpdateIdentifiers(sku, barcode, utcNow);
            UpdatedAtUtc = utcNow;
        }

        public void UpdateVariantSalesRules(
            Guid variantId,
            SalesUnit salesUnit,
            decimal minimumOrderQuantity,
            decimal quantityIncrement,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            GetVariant(variantId).UpdateSalesRules(
                salesUnit,
                minimumOrderQuantity,
                quantityIncrement,
                utcNow);
            UpdatedAtUtc = utcNow;
        }

        public void UpdateVariantShippingInformation(
            Guid variantId,
            Weight? shippingWeight,
            Dimensions? shippingDimensions,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            GetVariant(variantId).UpdateShippingInformation(
                shippingWeight,
                shippingDimensions,
                utcNow);

            UpdatedAtUtc = utcNow;
        }

        public void ChangeDefaultVariant(Guid variantId, DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            var variant = GetVariant(variantId);

            if (variant.IsDefault)
            {
                return;
            }

            RemoveCurrentDefault(utcNow);
            variant.MakeDefault(utcNow);
            UpdatedAtUtc = utcNow;
        }

        public void DeactivateVariant(Guid variantId, DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            GetVariant(variantId).Deactivate(utcNow);
            UpdatedAtUtc = utcNow;
        }

        public void ActivateVariant(Guid variantId, DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            GetVariant(variantId).Activate(utcNow);
            UpdatedAtUtc = utcNow;
        }

        public ProductImage AddImage(
            string url,
            string? altText,
            Guid? variantId,
            int displayOrder,
            bool makePrimary,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (_images.Count >= MaximumImageCount)
            {
                throw new DomainException(
                    $"A product cannot contain more than {MaximumImageCount} images.");
            }

            if (variantId.HasValue)
            {
                _ = GetVariant(variantId.Value);
            }

            var sameScopeImages = _images.Where(
                image => image.ProductVariantId == variantId);
            var shouldBePrimary = makePrimary || !sameScopeImages.Any();

            if (shouldBePrimary)
            {
                foreach (var image in sameScopeImages)
                {
                    image.RemovePrimary();
                }
            }

            var productImage = new ProductImage(
                Id,
                variantId,
                url,
                altText,
                displayOrder,
                shouldBePrimary);

            _images.Add(productImage);
            UpdatedAtUtc = utcNow;
            return productImage;
        }

        public void Publish(DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (!_variants.Any(variant => variant.IsActive))
            {
                throw new DomainException(
                    "A product requires at least one active variant before publication.");
            }

            if (_variants.Count(variant => variant.IsDefault && variant.IsActive) != 1)
            {
                throw new DomainException(
                    "A product requires exactly one active default variant.");
            }

            Status = ProductStatus.Active;
            PublishedAtUtc ??= utcNow;
            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductPublishedDomainEvent(Id, StoreId, utcNow));
        }

        public void MoveToDraft(DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            Status = ProductStatus.Draft;
            UpdatedAtUtc = utcNow;
        }

        public void Archive(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            if (Status == ProductStatus.Archived)
            {
                return;
            }

            Status = ProductStatus.Archived;
            ArchivedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new ProductArchivedDomainEvent(Id, StoreId, utcNow));
        }

        private List<ProductVariantSelection> CreateSelections(
            Guid variantId,
            IReadOnlyDictionary<Guid, Guid> selectedOptionValues,
            out string displayName,
            out string signature)
        {
            if (selectedOptionValues.Count != _options.Count)
            {
                throw new DomainException(
                    "A variant must select exactly one value from every product option.");
            }

            var selections = new List<ProductVariantSelection>(_options.Count);
            var displayParts = new List<string>(_options.Count);

            foreach (var option in _options.OrderBy(option => option.DisplayOrder))
            {
                if (!selectedOptionValues.TryGetValue(option.Id, out var valueId) ||
                    !option.ContainsValue(valueId))
                {
                    throw new DomainException(
                        $"A valid value is required for option '{option.Name}'.");
                }

                selections.Add(
                    new ProductVariantSelection(
                        variantId,
                        option.Id,
                        valueId));

                displayParts.Add(option.GetValueText(valueId));
            }

            displayName = displayParts.Count == 0
                ? "Default"
                : string.Join(" / ", displayParts);
            signature = BuildVariantSignature(selections);

            return selections;
        }


        private (string DisplayName, string Signature) ValidateAndDescribeSelections(IReadOnlyDictionary<Guid, Guid> selectedOptionValues)
        {
            if (selectedOptionValues.Count != _options.Count)
            {
                throw new DomainException(
                    "A variant must select exactly one value from every product option.");
            }

            var displayParts = new List<string>();

            var validatedSelections =
                new List<(Guid OptionId, Guid OptionValueId)>();

            foreach (var option in _options.OrderBy(
                         option => option.DisplayOrder))
            {
                if (!selectedOptionValues.TryGetValue(
                        option.Id,
                        out var optionValueId))
                {
                    throw new DomainException(
                        $"A value is required for option '{option.Name}'.");
                }

                if (!option.ContainsValue(optionValueId))
                {
                    throw new DomainException(
                        $"The selected value does not belong to option '{option.Name}'.");
                }

                validatedSelections.Add(
                    (
                        option.Id,
                        optionValueId
                    ));

                var valueText = option.GetValueText(optionValueId);

                displayParts.Add(valueText);
            }

            var displayName = displayParts.Count == 0
                ? "Default"
                : string.Join(" / ", displayParts);

            var signature = BuildVariantSignature(
                validatedSelections);

            return (displayName, signature);
        }


        private static string BuildVariantSignature(IEnumerable<(Guid OptionId, Guid OptionValueId)> selections)
        {
            return string.Join(
                "|",
                selections
                    .OrderBy(selection => selection.OptionId)
                    .Select(selection =>
                        $"{selection.OptionId:N}:{selection.OptionValueId:N}"));
        }

        private static string BuildVariantSignature(
            IEnumerable<ProductVariantSelection> selections)
        {
            return string.Join(
                "|",
                selections
                    .OrderBy(selection => selection.ProductOptionId)
                    .Select(selection =>
                        $"{selection.ProductOptionId:N}:{selection.ProductOptionValueId:N}"));
        }

        private ProductOption GetOption(Guid optionId)
        {
            Guard.NotEmpty(optionId, nameof(optionId));

            return _options.SingleOrDefault(option => option.Id == optionId)
                   ?? throw new DomainException(
                       $"Product option '{optionId}' was not found.");
        }

        private ProductVariant GetVariant(Guid variantId)
        {
            Guard.NotEmpty(variantId, nameof(variantId));

            return _variants.SingleOrDefault(variant => variant.Id == variantId)
                   ?? throw new DomainException(
                       $"Product variant '{variantId}' was not found.");
        }

        private void ValidateSkuIsUnique(
            string? sku,
            Guid? excludedVariantId = null)
        {
            var normalizedSku = Guard.OptionalText(
                sku,
                nameof(sku),
                ProductVariant.SkuMaxLength);

            if (normalizedSku is not null &&
                _variants.Any(variant =>
                    variant.Id != excludedVariantId &&
                    string.Equals(
                        variant.Sku,
                        normalizedSku,
                        StringComparison.OrdinalIgnoreCase)))
            {
                throw new DomainException(
                    $"SKU '{normalizedSku}' already exists in this product.");
            }
        }

        private void ValidateBarcodeIsUnique(
            string? barcode,
            Guid? excludedVariantId = null)
        {
            var normalizedBarcode = Guard.OptionalText(
                barcode,
                nameof(barcode),
                ProductVariant.BarcodeMaxLength);

            if (normalizedBarcode is not null &&
                _variants.Any(variant =>
                    variant.Id != excludedVariantId &&
                    string.Equals(
                        variant.Barcode,
                        normalizedBarcode,
                        StringComparison.OrdinalIgnoreCase)))
            {
                throw new DomainException(
                    $"Barcode '{normalizedBarcode}' already exists in this product.");
            }
        }

        private void ValidateCurrency(
            Money price,
            Guid? excludedVariantId = null)
        {
            var existingVariant = _variants.FirstOrDefault(
                variant => variant.Id != excludedVariantId);

            if (existingVariant is not null &&
                !existingVariant.Price.HasSameCurrency(price))
            {
                throw new DomainException(
                    "All variants of a product must use the same currency.");
            }
        }

        private void RemoveCurrentDefault(DateTime utcNow)
        {
            var currentDefault = _variants.SingleOrDefault(
                variant => variant.IsDefault);

            currentDefault?.RemoveDefault(utcNow);
        }

        private void EnsureNotArchived()
        {
            if (Status == ProductStatus.Archived)
            {
                throw new DomainException(
                    "An archived product cannot be modified.");
            }
        }

    }

}

