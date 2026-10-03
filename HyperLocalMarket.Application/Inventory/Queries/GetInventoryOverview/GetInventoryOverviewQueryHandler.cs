using HyperLocalMarket.Application.Inventory.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Queries.GetInventoryOverview
{
    public sealed class GetInventoryOverviewQueryHandler(
            IInventoryReader reader)
            : IRequestHandler<GetInventoryOverviewQuery, InventoryOverviewDto>
    {
        public Task<InventoryOverviewDto> Handle(
            GetInventoryOverviewQuery request,
            CancellationToken cancellationToken)
        {
            return reader.GetOverviewAsync(
                request.StoreId,
                request.UserId,
                request.Filter,
                cancellationToken);
        }
    }
}
