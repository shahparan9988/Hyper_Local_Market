using HyperLocalMarket.Api.Authentication;
using HyperLocalMarket.Shared.Exceptions;
using System.Security.Claims;

namespace HyperLocalMarket.Api.Common
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext context)
        {
            var value = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (Guid.TryParse(value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedException();
        }

        public static Guid GetSessionId(this HttpContext context)
        {
            var value = context.User.FindFirstValue(
                SessionAuthenticationDefaults.SessionIdClaimType);

            if (Guid.TryParse(value, out var sessionId))
            {
                return sessionId;
            }

            throw new UnauthorizedException();
        }

        public static Guid? TryGetUserId(this HttpContext context)
        {
            var value = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId) ? userId : null;

        }
    }
}
