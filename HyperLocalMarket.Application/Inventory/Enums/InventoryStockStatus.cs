using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Enums
{
    public enum InventoryStockStatus
    {
        InStock = 1,
        LowStock = 2,
        OutOfStock = 3,
        BackorderAvailable = 4,
        TrackingOff = 5,
        NeedsReview = 6
    }
}
