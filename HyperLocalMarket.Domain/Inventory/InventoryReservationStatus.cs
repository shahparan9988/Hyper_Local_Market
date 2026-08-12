using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Inventory
{
    public enum InventoryReservationStatus
    {
        Active = 1,
        Committed = 2,
        Released = 3,
        Expired = 4
    }
}
