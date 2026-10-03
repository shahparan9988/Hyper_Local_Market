using Amazon.S3;
using Amazon.S3.Model;
using HyperLocalMarket.Application.Images.Exceptions;
using HyperLocalMarket.Application.Images.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.External.Images
{
    public sealed class S3ImageStorage : IImageStorage
    {
        private readonly IAmazonS3 _s3;
        private readonly ImageStorageOptions _options;

        public S3ImageStorage(
            IAmazonS3 s3,
            IOptions<ImageStorageOptions> options)
        {
            _s3 = s3;
            _options = options.Value;
        }

        public string CreatePresignedUploadUrl(
            string objectKey,
            string contentType,
            DateTime expiresAtUtc)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                ContentType = contentType,
                Expires = expiresAtUtc,
                Protocol = Protocol.HTTPS
            };

            return _s3.GetPreSignedURL(request);
        }

        public async Task<DownloadedImage> DownloadAsync(
            string objectKey,
            CancellationToken cancellationToken)
        {
            using var response = await _s3.GetObjectAsync(
                new GetObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = objectKey
                },
                cancellationToken);

            const int maximumBytes = 10 * 1024 * 1024;
            if (response.ContentLength > maximumBytes)
                throw new ImageRejectedException("The uploaded image exceeds 10 MB.");
            await using var memory = new MemoryStream();
            var buffer = new byte[64 * 1024];
            int read;
            while ((read = await response.ResponseStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                if (memory.Length + read > maximumBytes)
                    throw new ImageRejectedException("The uploaded image exceeds 10 MB.");
                await memory.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }

            return new DownloadedImage(
                memory.ToArray(),
                response.ContentLength,
                response.Headers.ContentType ?? "application/octet-stream");
        }

        public async Task UploadProcessedAsync(
            string objectKey,
            byte[] content,
            string contentType,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream(content);

            await _s3.PutObjectAsync(
                new PutObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = objectKey,
                    InputStream = stream,
                    ContentType = contentType,
                    AutoCloseStream = false,
                    Headers =
                    {
                    CacheControl = "public,max-age=31536000,immutable"
                    }
                },
                cancellationToken);
        }

        public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
        {
            await _s3.DeleteObjectAsync(_options.BucketName, objectKey, cancellationToken);
        }

        public string CreatePublicUrl(string objectKey)
        {
            return
                $"{_options.CloudFrontBaseUrl.TrimEnd('/')}/" +
                objectKey;
        }
    }
}


