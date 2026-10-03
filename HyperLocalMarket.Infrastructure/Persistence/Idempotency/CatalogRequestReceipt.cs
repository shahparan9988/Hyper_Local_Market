using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Idempotency
{
    public sealed class CatalogRequestReceipt
    {
        public Guid StoreId { get; set; }
        public Guid UserId { get; set; }
        public string Operation { get; set; } = "";
        public Guid RequestKey { get; set; }
        public string Hash { get; set; } = "";
        public string ResultJson { get; set; } = "";
        public DateTime CreatedAtUtc { get; set; }
    }

}
