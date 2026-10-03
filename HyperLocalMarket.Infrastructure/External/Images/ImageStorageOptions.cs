using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.External.Images
{
    public sealed class ImageStorageOptions
    {
        public const string SectionName = "ImageStorage";

        public string BucketName { get; init; } = string.Empty;

        public string CloudFrontBaseUrl { get; init; } = string.Empty;
    }
}
