using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SetProductStatus
{
    public sealed record SetProductStatusCommand(
        Guid StoreId,
        Guid UserId,
        Guid ProductId,
        int ExpectedVersion,
        string Status) : IRequest<SellerProductDto>;
}
