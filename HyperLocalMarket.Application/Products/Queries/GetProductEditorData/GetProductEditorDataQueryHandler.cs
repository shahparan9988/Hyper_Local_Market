using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries.GetProductEditorData
{
    public sealed class GetProductEditorDataQueryHandler(IProductCatalogReader reader) : IRequestHandler<GetProductEditorDataQuery, ProductEditorDataDto>
    {
        public Task<ProductEditorDataDto> Handle(GetProductEditorDataQuery request, CancellationToken ct) => reader.EditorDataAsync(request.StoreId, request.UserId, ct);
    }
}
