using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SaveProduct
{
    public sealed record SaveProductCommand(Guid StoreId, Guid UserId, Guid? ProductId,
        Guid? IdempotencyKey, SaveProductInput Input) : IRequest<SellerProductDto>;
}
