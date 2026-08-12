using HyperLocalMarket.Application.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.Slugs
{
    public sealed class SlugGenerator : ISlugGenerator
    {
        public string Generate(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Value cannot be empty.",
                    nameof(value));
            }

            if (maxLength <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxLength));
            }

            var normalized = value
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormKC);

            var builder = new StringBuilder();

            var previousWasSeparator = false;

            foreach (var character in normalized)
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                    previousWasSeparator = false;

                    continue;
                }

                /*
                 * Spaces, punctuation, underscores,
                 * etc. become a single '-'.
                 */
                if (!previousWasSeparator &&
                    builder.Length > 0)
                {
                    builder.Append('-');
                    previousWasSeparator = true;
                }
            }

            var slug = builder
                .ToString()
                .Trim('-');

            /*
             * Example:
             *
             * "!!!"
             *
             * would otherwise result in an empty slug.
             */
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "product";
            }

            if (slug.Length > maxLength)
            {
                slug = slug[..maxLength]
                    .TrimEnd('-');
            }

            return slug;
        }
    }
}
