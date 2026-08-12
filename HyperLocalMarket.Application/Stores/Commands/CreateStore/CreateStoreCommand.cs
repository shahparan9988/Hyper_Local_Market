using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.CreateStore
{
    public sealed record CreateStoreCommand(
        Guid UserId,
        string Name,
        string? Description,
        string? PhoneNumber,
        string? Email,
        string TimeZoneId,
        StoreLocationDto Location
    ) : IRequest<Guid>;
}
