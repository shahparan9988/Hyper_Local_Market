using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HyperLocalMarket.Api.Authorization
{
    public sealed class PermissionPolicyProvider
        : DefaultAuthorizationPolicyProvider
    {
        public const string PolicyPrefix = "Permission:";

        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override Task<AuthorizationPolicy?> GetPolicyAsync(
            string policyName)
        {
            if (!policyName.StartsWith(
                    PolicyPrefix,
                    StringComparison.Ordinal))
            {
                return base.GetPolicyAsync(policyName);
            }

            var permission = policyName[PolicyPrefix.Length..];

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(
                    new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
    }
}
