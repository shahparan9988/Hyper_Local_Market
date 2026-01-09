using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace HyperLocalMarket.Application.Stores.Commands.CreateStore
{
    public sealed record CreateStoreCommand(
        string Name,
        string Street,
        string Suburb,
        string City,
        string State,
        string Postcode,
        string Country
    ) : IRequest<Guid>;

}
