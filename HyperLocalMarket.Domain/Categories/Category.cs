using HyperLocalMarket.Domain.Categories.Events;
using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories
{
    public sealed class Category : AggregateRoot
    {
        public const int NameMaxLength = 150;
        public const int SlugMaxLength = 180;
        public const int DescriptionMaxLength = 1_000;
        public const int MaximumAliasCount = 100;

        private readonly List<CategoryAlias> _aliases = [];

        private Category()
        {
        }

        private Category(
            Guid? parentCategoryId,
            string name,
            string slug,
            string? description,
            int displayOrder,
            DateTime utcNow)
        {
            ValidateNullableId(parentCategoryId, nameof(parentCategoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            if (parentCategoryId == Id)
            {
                throw new DomainException(
                    "A category cannot be its own parent.");
            }

            if (displayOrder < 0)
            {
                throw new DomainException(
                    "Category display order cannot be negative.");
            }

            ParentCategoryId = parentCategoryId;
            Name = Guard.RequiredText(name, nameof(name), NameMaxLength);
            Slug = ValidateSlug(slug);
            Description = Guard.OptionalText(
                description,
                nameof(description),
                DescriptionMaxLength);
            DisplayOrder = displayOrder;
            Status = CategoryStatus.Active;
            CreatedAtUtc = utcNow;
            UpdatedAtUtc = utcNow;
        }

        public Guid? ParentCategoryId { get; private set; }

        public string Name { get; private set; } = default!;

        public string Slug { get; private set; } = default!;

        public string? Description { get; private set; }

        public int DisplayOrder { get; private set; }

        public CategoryStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public IReadOnlyCollection<CategoryAlias> Aliases =>
            _aliases.AsReadOnly();

        public static Category Create(
            Guid? parentCategoryId,
            string name,
            string slug,
            string? description,
            int displayOrder,
            DateTime utcNow)
        {
            var category = new Category(
                parentCategoryId,
                name,
                slug,
                description,
                displayOrder,
                utcNow);

            category.AddDomainEvent(
                new CategoryCreatedDomainEvent(
                    category.Id,
                    parentCategoryId,
                    category.Name,
                    utcNow));

            return category;
        }

        public void UpdateDetails(
            string name,
            string slug,
            string? description,
            int displayOrder,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (displayOrder < 0)
            {
                throw new DomainException(
                    "Category display order cannot be negative.");
            }

            Name = Guard.RequiredText(name, nameof(name), NameMaxLength);
            Slug = ValidateSlug(slug);
            Description = Guard.OptionalText(
                description,
                nameof(description),
                DescriptionMaxLength);
            DisplayOrder = displayOrder;
            UpdatedAtUtc = utcNow;
        }

        public void ChangeParent(
            Guid? parentCategoryId,
            DateTime utcNow)
        {
            EnsureNotArchived();
            ValidateNullableId(parentCategoryId, nameof(parentCategoryId));
            Guard.Utc(utcNow, nameof(utcNow));

            if (parentCategoryId == Id)
            {
                throw new DomainException(
                    "A category cannot be its own parent.");
            }

            ParentCategoryId = parentCategoryId;
            UpdatedAtUtc = utcNow;
        }

        public CategoryAlias AddAlias(
            string name,
            string languageCode,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            if (_aliases.Count >= MaximumAliasCount)
            {
                throw new DomainException(
                    $"A category cannot contain more than {MaximumAliasCount} aliases.");
            }

            var normalizedName = Guard.RequiredText(
                name,
                nameof(name),
                CategoryAlias.NameMaxLength);
            var normalizedLanguageCode = Guard.RequiredText(
                languageCode,
                nameof(languageCode),
                CategoryAlias.LanguageCodeMaxLength);

            var duplicateExists = _aliases.Any(alias =>
                string.Equals(
                    alias.Name,
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    alias.LanguageCode,
                    normalizedLanguageCode,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                throw new DomainException(
                    $"Category already contains alias '{normalizedName}' " +
                    $"for language '{normalizedLanguageCode}'.");
            }

            var alias = new CategoryAlias(
                Id,
                normalizedName,
                normalizedLanguageCode);

            _aliases.Add(alias);
            UpdatedAtUtc = utcNow;

            return alias;
        }

        public void RemoveAlias(
            Guid aliasId,
            DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.NotEmpty(aliasId, nameof(aliasId));
            Guard.Utc(utcNow, nameof(utcNow));

            var alias = _aliases.SingleOrDefault(
                existingAlias => existingAlias.Id == aliasId);

            if (alias is null)
            {
                throw new DomainException(
                    $"Category alias '{aliasId}' was not found.");
            }

            _aliases.Remove(alias);
            UpdatedAtUtc = utcNow;
        }

        public void Activate(DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            ChangeStatus(CategoryStatus.Active, utcNow);
        }

        public void Hide(DateTime utcNow)
        {
            EnsureNotArchived();
            Guard.Utc(utcNow, nameof(utcNow));

            ChangeStatus(CategoryStatus.Hidden, utcNow);
        }

        public void Archive(DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));

            if (Status == CategoryStatus.Archived)
            {
                return;
            }

            ChangeStatus(CategoryStatus.Archived, utcNow);
        }

        private void ChangeStatus(
            CategoryStatus newStatus,
            DateTime utcNow)
        {
            var previousStatus = Status;

            if (previousStatus == newStatus)
            {
                return;
            }

            Status = newStatus;
            UpdatedAtUtc = utcNow;

            AddDomainEvent(
                new CategoryStatusChangedDomainEvent(
                    Id,
                    previousStatus,
                    newStatus,
                    utcNow));
        }

        private void EnsureNotArchived()
        {
            if (Status == CategoryStatus.Archived)
            {
                throw new DomainException(
                    "An archived category cannot be modified.");
            }
        }

        private static string ValidateSlug(string slug)
        {
            var normalizedSlug = Guard.RequiredText(
                    slug,
                    nameof(slug),
                    SlugMaxLength)
                .ToLowerInvariant();

            if (normalizedSlug.StartsWith('-') ||
                normalizedSlug.EndsWith('-') ||
                normalizedSlug.Contains("--", StringComparison.Ordinal) ||
                normalizedSlug.Any(character =>
                    !(character is >= 'a' and <= 'z') &&
                    !(character is >= '0' and <= '9') &&
                    character != '-'))
            {
                throw new DomainException(
                    "Category slug may contain only lowercase letters, numbers, " +
                    "and single hyphens.");
            }

            return normalizedSlug;
        }

        private static void ValidateNullableId(
            Guid? value,
            string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException(
                    $"{parameterName} cannot be an empty identifier.");
            }
        }

    }
}
