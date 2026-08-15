using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Application.Authorization.Queries.GetMyAccess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/auth/me")]
    public sealed class MyAccessController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MyAccessController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(MyAccessDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MyAccessDto>> Get(
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var result = await _mediator.Send(
                new GetMyAccessQuery(userId),
                cancellationToken);

            return Ok(result);
        }
    }
}
