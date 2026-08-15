using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Authorization.Queries.GetMyAccess
{
    public sealed record MyAccessDto(
        Guid UserId,
        bool IsBuyer,
        bool IsSeller,
        IReadOnlyList<string> PlatformRoles,
        IReadOnlyList<string> PlatformPermissions);
}
