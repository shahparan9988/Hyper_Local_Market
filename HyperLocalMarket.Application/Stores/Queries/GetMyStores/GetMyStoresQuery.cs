using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetMyStores
{
    public sealed record GetMyStoresQuery(
        Guid UserId) : IRequest<IReadOnlyList<MyStoreDto>>;
}
