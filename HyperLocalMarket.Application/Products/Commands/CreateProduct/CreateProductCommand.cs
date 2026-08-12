using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(
        Guid UserId,
        Guid StoreId,
        string Name,
        string? Description,
        string? BrandName) : IRequest<Guid>;
}
