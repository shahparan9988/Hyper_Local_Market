using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Common.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace HyperLocalMarket.Api.Authentication
{
    public sealed class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISessionRepository sessionRepository)
            : base(options, logger, encoder)
        {
            _sessionRepository = sessionRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var rawToken = Request.Cookies[SessionAuthenticationDefaults.CookieName];

            if (string.IsNullOrWhiteSpace(rawToken))
            {
                return AuthenticateResult.NoResult();
            }
            var tokenHash = SessionTokenGenerator.HashToken(rawToken);
            var session = await _sessionRepository
                .GetActiveSessionByTokenHashAsync(tokenHash, Context.RequestAborted);
            if (session is null)
            {
                return AuthenticateResult.Fail("Invalid session token.");
            }
            var claims = new[]
            {
                new Claim(SessionAuthenticationDefaults.SessionIdClaimType, session.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
