using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Me
{
    public sealed record GetCurrentUserQuery(
        Guid UserId
    ) : IRequest<CurrentUserResponse>;
}
