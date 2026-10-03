using HyperLocalMarket.Application.Inventory.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Queries.GetInventoryOverview
{
    public sealed record GetInventoryOverviewQuery(
        Guid StoreId,
        Guid UserId,
        InventoryOverviewFilter Filter)
        : IRequest<InventoryOverviewDto>;
}
