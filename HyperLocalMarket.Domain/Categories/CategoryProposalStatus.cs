using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Categories
{
    public enum CategoryProposalStatus
    {
        Pending = 1,
        Approved = 2,
        MappedToExisting = 3,
        Rejected = 4,
        Cancelled = 5
    }
}
