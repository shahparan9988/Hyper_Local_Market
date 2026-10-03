using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateContact
{
    public sealed record UpdateContactCommand(
        Guid StoreId,
        Guid UserId,
        string? PhoneNumber,
        string? Email,
        string? TimeZoneId)
        : IRequest<StoreContactDto?>;
}
