using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Api.Contracts.Auth;
using HyperLocalMarket.Application.Auth.Commands.Login;
using HyperLocalMarket.Application.Auth.Commands.Logout;
using HyperLocalMarket.Application.Auth.Commands.Register;
using HyperLocalMarket.Application.Auth.Queries.GetCurrentUser;
using HyperLocalMarket.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request,
            CancellationToken ct)
        {
            var deviceId = Request.Cookies["device_id"];

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = Guid.NewGuid().ToString("N");

                Response.Cookies.Append(
                    "device_id",
                    deviceId,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddYears(1)
                    });
            }

            Result<LoginResponse> result = await _sender.Send(
                new LoginCommand(
                    request.EmailOrPhone,
                    request.Password,
                    deviceId,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers.UserAgent.ToString()),
                ct);

            Response.Cookies.Append(
                "session_id",
                result.Value.RawSessionToken,
                CookieOptionsFactory.CreateSessionCookie(result.Value.ExpiresAtUtc));

            return Ok(result.Value);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userId = HttpContext.GetUserId();

            var result = await _sender.Send(
                new GetCurrentUserQuery(userId),
                ct);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userId = HttpContext.GetUserId();
            var sessionId = HttpContext.GetSessionId();

            await _sender.Send(
                new LogoutCommand(userId, sessionId),
                ct);

            Response.Cookies.Delete(
                "session_id",
                CookieOptionsFactory.DeleteSessionCookie());

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request,
            CancellationToken ct)
        {
            var result = await _sender.Send(
                new RegisterCommand(request.Email, request.Password, request.Phone, HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers.UserAgent.ToString()),
                ct);

            return CreatedAtAction(nameof(Me), new { id = result.UserId}, result);
        }
    }
}
