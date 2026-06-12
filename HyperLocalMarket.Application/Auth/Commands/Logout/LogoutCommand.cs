using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Logout
{
    public sealed record LogoutCommand(
        Guid UserId,
        Guid SessionId
    ) : IRequest;
}
