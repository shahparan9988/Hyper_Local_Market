using HyperLocalMarket.Application.Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HyperLocalMarket.Api.Authorization
{
    public sealed class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPlatformPermissionChecker _permissionChecker;

        public PermissionAuthorizationHandler(
            IPlatformPermissionChecker permissionChecker)
        {
            _permissionChecker = permissionChecker;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var rawUserId = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(rawUserId, out var userId))
            {
                return;
            }

            var hasPermission = await _permissionChecker
                .HasPermissionAsync(
                    userId,
                    requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
