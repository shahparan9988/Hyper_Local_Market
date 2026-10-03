using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus
{
    public sealed record GetStoreSetupStatusQuery(
    Guid StoreId,
    Guid UserId)
    : IRequest<StoreSetupStatusDto?>;
}
