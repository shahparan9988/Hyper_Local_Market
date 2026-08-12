using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Auth.Queries.GetCurrentUser
{
    public sealed record CurrentUserResponse(
        Guid UserId,
        string Email);
}
