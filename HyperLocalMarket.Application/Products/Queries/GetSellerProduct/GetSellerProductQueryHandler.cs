using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries.GetSellerProduct
{
    public sealed class GetSellerProductQueryHandler(IProductCatalogReader reader)
        : IRequestHandler<GetSellerProductQuery, SellerProductDto>
    {
        public Task<SellerProductDto> Handle(GetSellerProductQuery request, CancellationToken ct) => reader.GetAsync(request.StoreId, request.UserId, request.ProductId, ct);
    }

}
