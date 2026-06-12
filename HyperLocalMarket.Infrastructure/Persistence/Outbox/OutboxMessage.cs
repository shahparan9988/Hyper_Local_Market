using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime OccurredAtUtc { get; set; }
        public DateTime? ProcessedAtUtc { get; set; }
        public string? Error { get; set; }
        public int RetryCount { get; set; }
    }
}
