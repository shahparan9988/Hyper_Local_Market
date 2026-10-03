using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries.GetSellerProducts
{
    public sealed class GetSellerProductsQueryHandler(IProductCatalogReader reader) : IRequestHandler<GetSellerProductsQuery, ProductPageDto>
    {
        public Task<ProductPageDto> Handle(GetSellerProductsQuery request, CancellationToken ct) => reader.ListAsync(request.StoreId, request.UserId, request.Filter, ct);
    }
}
