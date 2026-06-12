using HyperLocalMarket.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.CreateStore
{
    public sealed record CreateStoreCommand(
        string Name,
        StoreLocationDto Location
    ) : IRequest<Guid>;

}
