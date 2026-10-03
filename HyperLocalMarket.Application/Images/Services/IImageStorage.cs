using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Services
{
    public sealed record DownloadedImage(
            byte[] Content,
            long ContentLength,
            string ContentType);

    public interface IImageStorage
    {
        string CreatePresignedUploadUrl(
            string objectKey,
            string contentType,
            DateTime expiresAtUtc);

        Task<DownloadedImage> DownloadAsync(
            string objectKey,
            CancellationToken cancellationToken);

        Task UploadProcessedAsync(
            string objectKey,
            byte[] content,
            string contentType,
            CancellationToken cancellationToken);

        Task DeleteAsync(string objectKey, CancellationToken cancellationToken);

        string CreatePublicUrl(string objectKey);
    }
}

