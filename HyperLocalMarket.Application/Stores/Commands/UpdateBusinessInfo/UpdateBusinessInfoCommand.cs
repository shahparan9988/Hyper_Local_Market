using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateBusinessInfo
{
    public sealed record UpdateBusinessInfoCommand(
        Guid StoreId,
        Guid UserId,
        string Name,
        string? Description) : IRequest<StoreBusinessInfoDto?>;
}
