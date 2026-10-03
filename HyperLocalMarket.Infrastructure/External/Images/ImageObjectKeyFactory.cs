using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Domain.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.External.Images
{
    public sealed class ImageObjectKeyFactory
        : IImageObjectKeyFactory
    {
        private const string IncomingProductsPrefix =
            "incoming/products/";

        private const string ProcessedProductsPrefix =
            "processed/products/";

        private const string IncomingStoresPrefix =
            "incoming/stores/";

        private const string ProcessedStoresPrefix =
            "processed/stores/";
        public string CreateIncomingStoreBrandingKey(
            Guid storeId,
            StoreBrandingImageKind kind,
            Guid uploadToken,
            string extension)
        {
            ValidateExtension(extension);

            var kindSegment =
                kind.ToString().ToLowerInvariant();

            return
                $"{IncomingStoresPrefix}" +
                $"{storeId:N}/branding/" +
                $"{kindSegment}/" +
                $"{uploadToken:N}" +
                $"{extension}";
        }

        public string CreateProcessedStoreBrandingKey(
            Guid storeId,
            StoreBrandingImageKind kind,
            Guid uploadToken,
            string extension)
        {
            ValidateExtension(extension);

            var kindSegment =
                kind.ToString().ToLowerInvariant();

            return
                $"{ProcessedStoresPrefix}" +
                $"{storeId:N}/branding/" +
                $"{kindSegment}/" +
                $"{uploadToken:N}" +
                $"{extension}";
        }
        public string CreateIncomingProductImageKey(
            Guid storeId,
            Guid assetId,
            string extension)
        {
            ValidateExtension(extension);
            return
                $"{IncomingProductsPrefix}" +
                $"{storeId:N}/" +
                $"{assetId:N}" +
                $"{extension}";
        }

        public string CreateProcessedProductImageKey(
            Guid storeId,
            Guid assetId)
        {
            return
                $"{ProcessedProductsPrefix}" +
                $"{storeId:N}/" +
                $"{assetId:N}.webp";
        }

        public bool IsIncomingProductImageKey(string key)
        {
            return key.StartsWith(
                IncomingProductsPrefix,
                StringComparison.Ordinal);
        }

        public bool IsProcessedProductImageKey(string key)
        {
            return key.StartsWith(
                ProcessedProductsPrefix,
                StringComparison.Ordinal);
        }

        public string CreateProcessedStoreLogo(Guid storeId)
            => $"processed/stores/{storeId:N}/logo.webp";

        public string CreateProcessedStoreCover(Guid storeId)
            => $"processed/stores/{storeId:N}/cover.webp";

        private static void ValidateExtension(
            string extension)
        {
            if (string.IsNullOrWhiteSpace(extension) ||
                !extension.StartsWith('.') ||
                extension.Contains('/') ||
                extension.Contains('\\'))
            {
                throw new ArgumentException(
                    "A valid file extension is required.",
                    nameof(extension));
            }
        }
    }
}
