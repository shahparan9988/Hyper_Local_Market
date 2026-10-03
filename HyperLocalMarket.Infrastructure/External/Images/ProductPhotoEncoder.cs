using HyperLocalMarket.Application.Images.Exceptions;
using HyperLocalMarket.Application.Images.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.External.Images
{
    public sealed class ProductPhotoEncoder : IProductPhotoEncoder
    {
        public async Task<ProcessedImage> EncodeAsync(
            byte[] content,
            string contentType,
            CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var format = Image.DetectFormat(content);

                if (!string.Equals(
                    format.DefaultMimeType,
                    contentType,
                    StringComparison.OrdinalIgnoreCase))
                        throw new ImageRejectedException("The file contents do not match its image type.");
                
                var options = new DecoderOptions 
                { 
                    MaxFrames = 2
                };

                var info = Image.Identify(options, content);

                if (info is null 
                    || info.Width <= 0 
                    || info.Height <= 0 
                    || (long)info.Width * info.Height > 40_000_000)
                {
                    throw new ImageRejectedException("Image dimensions are too large.");
                }

                using var image = Image.Load(options, content);

                if (image.Frames.Count > 1)
                    throw new ImageRejectedException("Animated product images are not supported.");

                image.Mutate(x => x.AutoOrient());

                if (image.Width > 1600 
                    || image.Height > 1600)
                        image.Mutate(x =>
                            x.Resize(new ResizeOptions {
                                Size = new Size(1600, 1600),
                                Mode = ResizeMode.Max 
                            }));

                image.Metadata.ExifProfile = null;
                image.Metadata.IptcProfile = null;
                image.Metadata.XmpProfile = null;

                await using var output = new MemoryStream();

                await image.SaveAsWebpAsync(
                    output,
                    new WebpEncoder { Quality = 82 },
                    cancellationToken);

                return new(output.ToArray(), "image/webp", ".webp");
            }
            catch (UnknownImageFormatException) 
            { 
                throw new ImageRejectedException("The file is not a supported image.");
            }
            catch (InvalidImageContentException) 
            { 
                throw new ImageRejectedException("The image is corrupted.");
            }
        }
    }

}
