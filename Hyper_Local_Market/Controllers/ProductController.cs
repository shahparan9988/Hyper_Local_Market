using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Api.Contracts.Products;
using HyperLocalMarket.Api.Contracts.Stores;
using HyperLocalMarket.Application.Images.Commands.CreateProductImageUpload;
using HyperLocalMarket.Application.Images.Dtos;
using HyperLocalMarket.Application.Images.Queries.GetProductImageStatus;
using HyperLocalMarket.Application.Products.Commands.AdjustProductInventory;
using HyperLocalMarket.Application.Products.Commands.CreateProduct;
using HyperLocalMarket.Application.Products.Commands.CreateStoreCategory;
using HyperLocalMarket.Application.Products.Commands.SaveProduct;
using HyperLocalMarket.Application.Products.Commands.SetProductAvailability;
using HyperLocalMarket.Application.Products.Commands.SetProductStatus;
using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Application.Products.Queries.GetProductEditorData;
using HyperLocalMarket.Application.Products.Queries.GetSellerProduct;
using HyperLocalMarket.Application.Products.Queries.GetSellerProducts;
using HyperLocalMarket.Application.Stores.Commands.DeliveryOptions;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Shared.Exceptions;
using HyperLocalMarket.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/stores/{storeId:guid}/products")]
    public sealed class ProductController(IMediator mediator) : ControllerBase
    {
        [HttpGet("editor-data")]
        public async Task<ActionResult<ProductEditorDataDto>> EditorData(
            Guid storeId,
            CancellationToken ct) =>
            Ok(await mediator.Send(new GetProductEditorDataQuery(storeId, HttpContext.GetUserId()), ct));

        [HttpGet]
        public async Task<ActionResult<ProductPageDto>> List(Guid storeId, CancellationToken ct,
            [FromQuery] string search = "", [FromQuery] Guid? categoryId = null,
            [FromQuery] string status = "Current", [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
            Ok(await mediator.Send(new GetSellerProductsQuery(storeId, HttpContext.GetUserId(), new(search, categoryId, status, page, pageSize)), ct));

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<SellerProductDto>> Get(Guid storeId, Guid productId, CancellationToken ct) =>
            Ok(await mediator.Send(new GetSellerProductQuery(storeId, HttpContext.GetUserId(), productId), ct));

        [HttpPost]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<ActionResult<SellerProductDto>> Create(Guid storeId, [FromBody] SaveProductRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new SaveProductCommand(storeId, HttpContext.GetUserId(), null, RequestKey(), request.ToInput()), ct);
            return CreatedAtAction(nameof(Get), new { storeId, productId = result.Id }, result);
        }

        [HttpPut("{productId:guid}")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<ActionResult<SellerProductDto>> Save(Guid storeId, Guid productId, [FromBody] SaveProductRequest request, CancellationToken ct) =>
            Ok(await mediator.Send(new SaveProductCommand(storeId, HttpContext.GetUserId(), productId, null, request.ToInput()), ct));

        [HttpPatch("{productId:guid}/status")]
        public async Task<ActionResult<SellerProductDto>> Status(Guid storeId, Guid productId, [FromBody] SetProductStatusRequest request, CancellationToken ct) =>
            Ok(await mediator.Send(new SetProductStatusCommand(storeId, HttpContext.GetUserId(), productId, request.ExpectedVersion, request.Status), ct));

        [HttpPatch("{productId:guid}/availability")]
        public async Task<ActionResult<SellerProductDto>> Availability(Guid storeId, Guid productId, [FromBody] SetProductAvailabilityRequest request, CancellationToken ct) =>
            Ok(await mediator.Send(new SetProductAvailabilityCommand(storeId, HttpContext.GetUserId(), productId, request.ExpectedVersion, request.Availability), ct));

        [HttpPut("{productId:guid}/inventory")]
        public async Task<ActionResult<SellerProductDto>> Inventory(Guid storeId, Guid productId, [FromBody] AdjustProductInventoryRequest request, CancellationToken ct) =>
            Ok(await mediator.Send(new AdjustProductInventoryCommand(storeId, HttpContext.GetUserId(), productId,
                new(request.ExpectedVersion, request.Reason, request.Variants)), ct));

        [HttpPost("~/api/stores/{storeId:guid}/product-categories")]
        public async Task<ActionResult<StoreCategoryDto>> CreateCategory(Guid storeId, [FromBody] CreateStoreProductCategoryRequest request, CancellationToken ct) =>
            StatusCode(StatusCodes.Status201Created, await mediator.Send(new CreateStoreCategoryCommand(storeId, HttpContext.GetUserId(), RequestKey(), request.Name, request.ParentId), ct));

        [HttpPost("~/api/stores/{storeId:guid}/product-images/upload-requests")]
        public async Task<ActionResult<ProductImageUploadDto>> RequestImage(Guid storeId, [FromBody] CreateProductImageUploadRequest request, CancellationToken ct) =>
            Ok(await mediator.Send(new CreateProductImageUploadCommand(storeId, HttpContext.GetUserId(), request.FileName, request.ContentType, request.FileSizeBytes), ct));

        [HttpGet("~/api/stores/{storeId:guid}/product-images/uploads/{uploadId:guid}")]
        public async Task<ActionResult<ProductImageStatusDto>> ImageStatus(Guid storeId, Guid uploadId, CancellationToken ct) =>
            Ok(await mediator.Send(new GetProductImageStatusQuery(storeId, HttpContext.GetUserId(), uploadId), ct));

        // Preserve your existing delivery-selection endpoints and response types.
        [HttpGet("{productId:guid}/delivery-options")]
        public async Task<ActionResult<ProductDeliverySelectionDto>> GetDeliveryOptions(Guid storeId, Guid productId, CancellationToken ct) =>
            Ok(await mediator.Send(new GetProductDeliverySelectionQuery(storeId, HttpContext.GetUserId(), productId), ct));

        [HttpPut("{productId:guid}/delivery-options")]
        public async Task<ActionResult<ProductDeliverySelectionDto>> SaveDeliveryOptions(Guid storeId, Guid productId,
            [FromBody] SaveProductDeliverySelectionRequest request, CancellationToken ct) =>
            Ok(await mediator.Send(new SaveProductDeliverySelectionCommand(storeId, HttpContext.GetUserId(), productId, request.ExpectedVersion, request.DeliveryOptionIds), ct));

        [AllowAnonymous]
        [HttpGet("~/api/products/{productId:guid}/delivery-options")]
        public async Task<ActionResult<PublicProductDeliveryDto>> PublicDeliveryOptions(Guid productId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetPublicProductDeliveryQuery(productId), ct);
            return result is null ? NotFound(new { message = "Product was not found." }) : Ok(result);
        }
        private Guid RequestKey()
        {
            if (!Guid.TryParse(Request.Headers["Idempotency-Key"].ToString(), out var key) || key == Guid.Empty)
                throw new DomainException("Supply a UUID in the Idempotency-Key header.");
            return key;
        }
    }
}
