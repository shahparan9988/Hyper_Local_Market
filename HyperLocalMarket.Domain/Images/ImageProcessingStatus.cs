using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Images
{
    public enum ImageProcessingStatus
    {
        PendingUpload = 1,
        Processing = 2,
        Ready = 3,
        Rejected = 4,
        Failed = 5,
        Removed = 6
    }
}
