using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.PublishStore
{
    public sealed record PublishStoreCommand(Guid StoreId, Guid UserId)
        : IRequest<StorePublicationDto>;

}
