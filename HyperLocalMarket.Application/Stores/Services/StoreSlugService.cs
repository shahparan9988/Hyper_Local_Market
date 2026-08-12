using HyperLocalMarket.Application.Stores.Services;
using System.Globalization;
using System.Text;

namespace HyperLocalMarket.Infrastructure.Services
{
    public sealed class StoreSlugService : IStoreSlugService
    {
        private const int SlugMaxLength = 200;

        public StoreSlugService(){}

        public string GenerateUnique(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                throw new ArgumentException(
                    "Store name is required.",
                    nameof(storeName));
            }

            var baseSlug = GenerateBaseSlug(storeName);
            var uniqueSuffix = Guid.NewGuid()
                .ToString("N")[..16];

            var availableBaseLength =
                SlugMaxLength - uniqueSuffix.Length - 1;

            if (baseSlug.Length > availableBaseLength)
            {
                baseSlug = baseSlug[..availableBaseLength]
                    .TrimEnd('-');
            }

            return $"{baseSlug}-{uniqueSuffix}";
        }

        private static string GenerateBaseSlug(string storeName)
        {
            var normalizedName = storeName
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();
            var previousCharacterWasSeparator = false;

            foreach (var character in normalizedName)
            {
                var unicodeCategory =
                    CharUnicodeInfo.GetUnicodeCategory(character);

                if (unicodeCategory ==
                    UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (IsAsciiLetterOrDigit(character))
                {
                    builder.Append(character);
                    previousCharacterWasSeparator = false;

                    continue;
                }

                if (builder.Length > 0 &&
                    !previousCharacterWasSeparator)
                {
                    builder.Append('-');
                    previousCharacterWasSeparator = true;
                }
            }

            var slug = builder
                .ToString()
                .Trim('-');

            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "store";
            }

            if (slug.Length > SlugMaxLength)
            {
                slug = slug[..SlugMaxLength]
                    .TrimEnd('-');
            }

            return slug;
        }

        private static bool IsAsciiLetterOrDigit(
            char character)
        {
            return character is >= 'a' and <= 'z' ||
                   character is >= '0' and <= '9';
        }
    }
}
