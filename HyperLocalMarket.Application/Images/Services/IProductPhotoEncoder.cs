using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Services
{
    public interface IProductPhotoEncoder
    {
        Task<ProcessedImage> EncodeAsync(byte[] content, string contentType, CancellationToken ct);
    }
    public interface IProductImageCleanup
    {
        Task CleanupAsync(CancellationToken ct);
    }
}
