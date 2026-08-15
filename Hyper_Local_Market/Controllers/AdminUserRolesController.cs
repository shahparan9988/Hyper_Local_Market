using HyperLocalMarket.Api.Authorization;
using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Application.Authorization.Commands.AssignPlatformRole;
using HyperLocalMarket.Application.Authorization.Commands.RevokePlatformRole;
using HyperLocalMarket.Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Route("api/admin/users/{userId:guid}/platform-roles")]
    public class AdminUserRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminUserRolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{roleCode}")]
        [HasPermission(PlatformPermissionCodes.RolesAssign)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Assign(
            [FromRoute] Guid userId,
            [FromRoute] string roleCode,
            CancellationToken cancellationToken)
        {
            var actorUserId = HttpContext.GetUserId();

            await _mediator.Send(
                new AssignPlatformRoleCommand(
                    actorUserId,
                    userId,
                    roleCode),
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{roleCode}")]
        [HasPermission(PlatformPermissionCodes.RolesAssign)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Revoke(
            [FromRoute] Guid userId,
            [FromRoute] string roleCode,
            CancellationToken cancellationToken)
        {
            var actorUserId = HttpContext.GetUserId();

            await _mediator.Send(
                new RevokePlatformRoleCommand(
                    actorUserId,
                    userId,
                    roleCode),
                cancellationToken);

            return NoContent();
        }
    }
}
