using HyperLocalMarket.Domain.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Services
{
    public sealed record ProcessedImage(
        byte[] Content,
        string ContentType,
        string Extension);

    public interface IStoreBrandingImageProcessor
    {
        Task<ProcessedImage> ProcessAsync(
            byte[] originalContent,
            StoreBrandingImageKind kind,
            CancellationToken cancellationToken);
    }
}
