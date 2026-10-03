using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries.GetSellerProducts
{
    public sealed record GetSellerProductsQuery(Guid StoreId, Guid UserId, ProductListFilter Filter) : IRequest<ProductPageDto>;
}
