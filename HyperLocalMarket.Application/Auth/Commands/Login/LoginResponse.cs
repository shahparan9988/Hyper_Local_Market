using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Commands.Login
{
    public sealed record LoginResponse
    (
        Guid UserId,
        Guid SessionId,
        string RawSessionToken,
        DateTime ExpiresAtUtc
    );
}
