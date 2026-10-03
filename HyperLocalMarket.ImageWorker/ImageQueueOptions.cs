using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.ImageWorker
{
    public sealed class ImageQueueOptions
    {
        public const string SectionName = "ImageQueue";

        public string QueueUrl { get; init; } = string.Empty;

        public int MaximumReceiveCount { get; init; } = 5;
    }
}
