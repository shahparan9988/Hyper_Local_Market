using Microsoft.AspNetCore.Authorization;

namespace HyperLocalMarket.Api.Authorization
{
    public sealed record PermissionRequirement(
        string Permission) : IAuthorizationRequirement;
}
