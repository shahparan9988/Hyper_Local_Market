using HyperLocalMarket.Application.Inventory.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Queries
{
    public interface IInventoryReader
    {
        Task<InventoryOverviewDto> GetOverviewAsync(
            Guid storeId,
            Guid userId,
            InventoryOverviewFilter filter,
            CancellationToken cancellationToken);
    }
}
