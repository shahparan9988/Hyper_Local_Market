using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HyperLocalMarket.ImageWorker
{
    public sealed class S3EventEnvelope
    {
        [JsonPropertyName("Records")]
        public List<S3EventRecord> Records { get; init; } = [];
    }

    public sealed class S3EventRecord
    {
        [JsonPropertyName("eventName")]
        public string EventName { get; init; } = string.Empty;

        [JsonPropertyName("s3")]
        public S3EventData S3 { get; init; } = new();
    }

    public sealed class S3EventData
    {
        [JsonPropertyName("object")]
        public S3ObjectData Object { get; init; } = new();

        [JsonPropertyName("bucket")]
        public S3BucketData Bucket { get; init; } = new();
    }

    public sealed class S3ObjectData
    {
        [JsonPropertyName("key")]
        public string Key { get; init; } = string.Empty;
    }

    public sealed class S3BucketData
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";
    }

}
        

