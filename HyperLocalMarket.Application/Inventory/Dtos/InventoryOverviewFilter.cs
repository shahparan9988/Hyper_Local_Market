using HyperLocalMarket.Application.Inventory.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Dtos
{
    public sealed record InventoryOverviewFilter(
            string? Search = null,
            Guid? CategoryId = null,
            bool? TrackInventory = null,
            InventoryStockStatus? StockStatus = null,
            bool IncludeArchived = true,
            int Page = 1,
            int PageSize = 20);
}
