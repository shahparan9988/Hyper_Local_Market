using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Images
{
    public static class ProductImageRules
    {
        public const long MaximumBytes = 10 * 1024 * 1024;

        public static void ValidateUpload(
            string contentType,
            long size)
        {
            _ = GetExtension(contentType);

            if (size <= 0 || size > MaximumBytes)
            {
                throw new DomainException(
                    "The image must be between 1 byte and 10 MB.");
            }
        }

        public static string GetExtension(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",

                _ => throw new DomainException(
                    "Only JPEG, PNG and WebP are allowed.")
            };
        }
    }
}
