using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.SetAcceptingOrders
{
    public sealed record SetAcceptingOrdersCommand(
        Guid StoreId, Guid UserId, bool IsAcceptingOrders)
        : IRequest<StorePublicationDto>;
}
