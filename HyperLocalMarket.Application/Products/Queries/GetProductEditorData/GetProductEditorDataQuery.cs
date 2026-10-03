using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries.GetProductEditorData
{
    public sealed record GetProductEditorDataQuery(
        Guid StoreId,
        Guid UserId) : IRequest<ProductEditorDataDto>;
}
