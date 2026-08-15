using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Queries.GetMyAccess
{
    public sealed record GetMyAccessQuery(
        Guid UserId) : IRequest<MyAccessDto>;
}
