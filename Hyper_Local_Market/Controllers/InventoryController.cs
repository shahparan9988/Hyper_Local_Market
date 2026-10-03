using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Api.Contracts.Inventory;
using HyperLocalMarket.Application.Inventory.Dtos;
using HyperLocalMarket.Application.Inventory.Enums;
using HyperLocalMarket.Application.Inventory.Queries.GetInventoryOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/stores/{storeId:guid}/inventory")]
    public sealed class InventoryController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(InventoryOverviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryOverviewDto>> GetOverview(
            Guid storeId,
            [FromQuery] GetInventoryOverviewRequest request,
            CancellationToken cancellationToken)
        {
            var filter = new InventoryOverviewFilter(
                request.Search,
                request.CategoryId,
                request.TrackInventory,
                request.StockStatus,
                request.IncludeArchived,
                request.Page,
                request.PageSize);
            var result = await mediator.Send(
                new GetInventoryOverviewQuery(
                    storeId,
                    HttpContext.GetUserId(),
                    filter),
                cancellationToken);

            return Ok(result);
        }
    }
}
