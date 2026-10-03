using HyperLocalMarket.Application.Images.Exceptions;
using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Domain.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.External.Images
{
    public sealed class StoreBrandingImageProcessor
    : IStoreBrandingImageProcessor
    {
        private const long MaximumPixelCount = 40_000_000;

        public async Task<ProcessedImage> ProcessAsync(
            byte[] originalContent,
            StoreBrandingImageKind kind,
            CancellationToken cancellationToken)
        {
            try
            {
                var information = Image.Identify(originalContent);

                if (information is null)
                {
                    throw new ImageRejectedException(
                        "The uploaded file is not a recognizable image.");
                }

                var pixelCount =
                    (long)information.Width * information.Height;

                if (pixelCount > MaximumPixelCount)
                {
                    throw new ImageRejectedException(
                        "The image dimensions are too large.");
                }

                using var image = Image.Load(originalContent);

                if (image.Frames.Count > 1)
                {
                    throw new ImageRejectedException(
                        "Animated images are not supported.");
                }

                var targetSize = kind switch
                {
                    StoreBrandingImageKind.Logo =>
                        new Size(512, 512),

                    StoreBrandingImageKind.Cover =>
                        new Size(1600, 600),

                    _ => throw new ImageRejectedException(
                        "Unsupported store image kind.")
                };

                image.Mutate(context => context
                    .AutoOrient()
                    .Resize(new ResizeOptions
                    {
                        Size = targetSize,
                        Mode = ResizeMode.Crop,
                        Position = AnchorPositionMode.Center
                    }));

                await using var output = new MemoryStream();

                await image.SaveAsWebpAsync(
                    output,
                    new WebpEncoder
                    {
                        Quality = 82
                    },
                    cancellationToken);

                return new ProcessedImage(
                    output.ToArray(),
                    "image/webp",
                    ".webp");
            }
            catch (ImageRejectedException)
            {
                throw;
            }
            catch (UnknownImageFormatException)
            {
                throw new ImageRejectedException(
                    "The uploaded file is not a supported image.");
            }
            catch (InvalidImageContentException)
            {
                throw new ImageRejectedException(
                    "The uploaded image is corrupted.");
            }
        }
    }
}
