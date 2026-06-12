using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Common.Security;

namespace HyperLocalMarket.Api.Middlewares
{
    public sealed class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ISessionRepository sessionRepository)
        {
            var rawToken = context.Request.Cookies["session_id"];

            if (!string.IsNullOrWhiteSpace(rawToken))
            {
                var tokenHash = SessionTokenGenerator.HashToken(rawToken);

                var session = await sessionRepository
                    .GetActiveSessionByTokenHashAsync(
                        tokenHash,
                        context.RequestAborted);

                if (session is not null)
                {
                    context.Items["UserId"] = session.UserId;
                    context.Items["SessionId"] = session.Id;
                }
            }

            await _next(context);
        }
    }
}
