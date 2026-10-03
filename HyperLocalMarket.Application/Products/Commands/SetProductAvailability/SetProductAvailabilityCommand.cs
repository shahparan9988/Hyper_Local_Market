using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SetProductAvailability
{
    public sealed record SetProductAvailabilityCommand(
        Guid StoreId,
        Guid UserId,
        Guid ProductId,
        int ExpectedVersion,
        string Availability) : IRequest<SellerProductDto>;
}
