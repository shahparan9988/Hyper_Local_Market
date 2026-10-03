using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries.GetSellerProduct
{
    public sealed record GetSellerProductQuery(
        Guid StoreId,
        Guid UserId,
        Guid ProductId) : IRequest<SellerProductDto>;
}
