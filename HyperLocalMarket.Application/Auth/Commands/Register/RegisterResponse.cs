using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Register
{
    public sealed record RegisterResponse(
        Guid UserId,
        string Email);
}
