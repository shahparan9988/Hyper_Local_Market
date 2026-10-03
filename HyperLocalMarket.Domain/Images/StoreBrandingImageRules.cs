using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images
{
    public static class StoreBrandingImageRules
    {
        public const long MaximumUploadBytes = 10 * 1024 * 1024;

        public static readonly IReadOnlySet<string> AllowedContentTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
            "image/jpeg",
            "image/png",
            "image/webp"
            };

        public static string GetExtension(string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(contentType),
                    contentType,
                    "Unsupported image content type.")
            };
        }
    }
}
