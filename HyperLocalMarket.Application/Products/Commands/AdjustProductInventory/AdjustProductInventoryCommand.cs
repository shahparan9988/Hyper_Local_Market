using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.AdjustProductInventory
{
    public sealed record AdjustProductInventoryCommand(
        Guid StoreId,
        Guid UserId,
        Guid ProductId,
        InventoryAdjustmentInput Input) : IRequest<SellerProductDto>;
}
