using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public enum CategoryAssignmentStatus
    {
        Unassigned = 1,
        PendingReview = 2,
        Confirmed = 3,
        NeedsCorrection = 4
    }
}
