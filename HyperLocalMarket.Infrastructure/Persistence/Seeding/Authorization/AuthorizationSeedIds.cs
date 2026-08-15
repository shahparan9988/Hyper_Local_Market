using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Seeding.Authorization
{
    internal static class AuthorizationSeedIds
    {
        internal static readonly Guid PlatformAdminRole =
        Guid.Parse("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01");

        internal static readonly Guid CategoryModeratorRole =
            Guid.Parse("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a02");

        internal static readonly Guid RolesAssignPermission =
            Guid.Parse("2a74d1b6-ce4d-4456-ad23-587cde13b101");

        internal static readonly Guid UsersViewPermission =
            Guid.Parse("2a74d1b6-ce4d-4456-ad23-587cde13b102");

        internal static readonly Guid UsersSuspendPermission =
            Guid.Parse("2a74d1b6-ce4d-4456-ad23-587cde13b103");

        internal static readonly Guid StoresSuspendPermission =
            Guid.Parse("2a74d1b6-ce4d-4456-ad23-587cde13b104");

        internal static readonly Guid CategoriesReviewPermission =
            Guid.Parse("2a74d1b6-ce4d-4456-ad23-587cde13b105");

        internal static readonly Guid CategoriesManagePermission =
            Guid.Parse("2a74d1b6-ce4d-4456-ad23-587cde13b106");

        internal static readonly DateTime CreatedAtUtc =
            new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
