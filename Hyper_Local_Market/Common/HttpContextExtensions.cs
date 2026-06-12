using HyperLocalMarket.Shared.Exceptions;

namespace HyperLocalMarket.Api.Common
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext context)
        {
            if (context.Items["UserId"] is Guid userId)
                return userId;

            throw new UnauthorizedException();
        }

        public static Guid GetSessionId(this HttpContext context)
        {
            if (context.Items["SessionId"] is Guid sessionId)
                return sessionId;

            throw new UnauthorizedException();
        }
    }
}
