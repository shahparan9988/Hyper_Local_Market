using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Api.Contracts.Products;
using HyperLocalMarket.Application.Products.Commands.CreateProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/stores/{storeId:guid}/products")]
    public sealed class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<CreateProductResponse>> Create(
        [FromRoute] Guid storeId,
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var command = new CreateProductCommand(
                UserId: userId,
                StoreId: storeId,
                Name: request.Name,
                Description: request.Description,
                BrandName: request.BrandName);

            var productId = await _mediator.Send(
                command,
                cancellationToken);

            var response = new CreateProductResponse(
                productId);

            return Created(
                $"/api/stores/{storeId}/products/{productId}",
                response);
        }

    }
}
