using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Api.Contracts.Stores;
using HyperLocalMarket.Application.Images.Commands.CreateStoreBrandingUpload;
using HyperLocalMarket.Application.Images.Commands.RemoveStoreBrandingImage;
using HyperLocalMarket.Application.Images.Queries.GetStoreBrandingUploadStatus;
using HyperLocalMarket.Application.Stores.Commands.CreateStore;
using HyperLocalMarket.Application.Stores.Commands.DeliveryOptions;
using HyperLocalMarket.Application.Stores.Commands.PublishStore;
using HyperLocalMarket.Application.Stores.Commands.SetAcceptingOrders;
using HyperLocalMarket.Application.Stores.Commands.UpdateBusinessHours;
using HyperLocalMarket.Application.Stores.Commands.UpdateBusinessInfo;
using HyperLocalMarket.Application.Stores.Commands.UpdateContact;
using HyperLocalMarket.Application.Stores.Commands.UpdateLocation;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Queries.GetMyStores;
using HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus;
using HyperLocalMarket.Domain.Images;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class StoreController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStoreRequest request, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var command = new CreateStoreCommand(
                userId,
                request.Name,
                request.Description,
                request.PhoneNumber,
                request.Email,
                request.TimeZoneId,
                new StoreLocationDto(
                    request.Location.CountryCode,
                    new GeoLocationDto(request.Location.Coordinates.Latitude, request.Location.Coordinates.Longitude),
                    new PostalAddressDto(
                        request.Location.Address.AddressLine1,
                        request.Location.Address.AddressLine2,
                        request.Location.Address.Locality,
                        request.Location.Address.Region,
                        request.Location.Address.Postcode,
                        request.Location.Address.Landmark
                    ),
                    new AdminAreaDto(
                        request.Location.AdminArea.Level1Id,
                        request.Location.AdminArea.Level2Id,
                        request.Location.AdminArea.Level3Id,
                        request.Location.AdminArea.Level1,
                        request.Location.AdminArea.Level2,
                        request.Location.AdminArea.Level3,
                        request.Location.AdminArea.Level4
                    )
                )
            );

            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [Authorize]
        [HttpGet("{storeId:guid}/setup-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<StoreSetupStatusDto>> GetSetupStatus(
            Guid storeId,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var result = await _mediator.Send(
                new GetStoreSetupStatusQuery(
                    storeId,
                    userId),
                cancellationToken);

            if (result is null)
            {
                // The same result is returned when the store does not
                // exist or belongs to another user.
                return NotFound(new
                {
                    message = "Store was not found."
                });
            }

            return Ok(result);
        }


        [Authorize]
        [HttpPut("{storeId:guid}/business-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<StoreBusinessInfoDto>> UpdateBusinessInfo(
            Guid storeId,
            [FromBody] UpdateBusinessInfoRequest request,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var command = new UpdateBusinessInfoCommand(
                storeId,
                userId,
                request.Name ?? string.Empty,
                request.Description);

            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Store not found."
                });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("mine")]
        [ProducesResponseType(
        typeof(IReadOnlyList<MyStoreDto>),
        StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<MyStoreDto>>> GetMine(CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var stores = await _mediator.Send(
                new GetMyStoresQuery(userId),
                cancellationToken);

            return Ok(stores);
        }

        [Authorize]
        [HttpPut("{storeId:guid}/location")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UpdateStoreLocationResultDto>> UpdateLocation(
            Guid storeId,
            [FromBody] StoreLocationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var command = new UpdateLocationCommand(
                storeId,
                userId,
                request.CountryCode,
                request.Coordinates?.Latitude,
                request.Coordinates?.Longitude,
                request.Address?.AddressLine1,
                request.Address?.AddressLine2,
                request.Address?.Locality,
                request.Address?.Region,
                request.Address?.Postcode,
                request.Address?.Landmark,
                request.AdminArea?.Level1Id,
                request.AdminArea?.Level2Id,
                request.AdminArea?.Level3Id,
                request.AdminArea?.Level1,
                request.AdminArea?.Level2,
                request.AdminArea?.Level3,
                request.AdminArea?.Level4);

            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Store not found."
                });
            }

            return Ok(result);
        }


        [Authorize]
        [HttpPut("{storeId:guid}/contact")]
        [ProducesResponseType(
            typeof(StoreContactDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoreContactDto>> UpdateContact(
            Guid storeId,
            [FromBody]
            UpdateStoreContactRequest request,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var result = await _mediator.Send(
                new UpdateContactCommand(
                    storeId,
                    userId,
                    request.PhoneNumber,
                    request.Email,
                    request.TimeZoneId),
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Store was not found."
                });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{storeId:guid}/branding/{kind}/upload-request")]
        [ProducesResponseType(
            typeof(StoreContactDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateStoreBrandingUploadResult>> CreateBrandingUploadRequest(
            Guid storeId,
            string kind,
            [FromBody] CreateBrandingUploadRequest request,
            CancellationToken cancellationToken)
        {
            if (!TryParseBrandingKind(kind, out var imageKind))
            {
                return BadRequest(new
                {
                    message = "Branding image kind must be logo or cover."
                });
            }

            var userId = HttpContext.GetUserId();

            var result = await _mediator.Send(
                new CreateStoreBrandingUploadCommand(
                    storeId,
                    userId,
                    imageKind,
                    request.FileName,
                    request.ContentType,
                    request.FileSizeBytes),
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Store was not found."
                });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{storeId:guid}/branding/{kind}")]
        [ProducesResponseType(
            typeof(StoreContactDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoreBrandingDto>> RemoveBrandingImage(
            Guid storeId,
            string kind,
            CancellationToken cancellationToken)
        {
            if (!TryParseBrandingKind(kind, out var imageKind))
            {
                return BadRequest(new
                {
                    message = "Branding image kind must be logo or cover."
                });
            }

            var userId = HttpContext.GetUserId();

            var result = await _mediator.Send(
                new RemoveStoreBrandingImageCommand(
                    storeId,
                    userId,
                    imageKind),
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Store was not found."
                });
            }

            return Ok(result);
        }


        [Authorize]
        [HttpGet("{storeId:guid}/branding/uploads/{uploadId:guid}")]
        [ProducesResponseType(
            typeof(StoreContactDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoreBrandingUploadStatusDto>> GetBrandingUploadStatus(
            Guid storeId,
            Guid uploadId,
            CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var result = await _mediator.Send(
                new GetStoreBrandingUploadStatusQuery(
                    storeId,
                    userId,
                    uploadId),
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Branding upload was not found."
                });
            }

            return Ok(result);
        }


        [Authorize]
        [HttpPut("{storeId:guid}/business-hours")]
        [ProducesResponseType(
        typeof(StoreBusinessHoursDto),
        StatusCodes.Status200OK)]
        [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
        StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoreBusinessHoursDto>>
            UpdateBusinessHours(
                Guid storeId,
                [FromBody] UpdateBusinessHoursRequest request,
                CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();

            var command = new UpdateBusinessHoursCommand(
                StoreId: storeId,
                UserId: userId,
                Hours: request.Hours
                    .Select(day =>
                        new BusinessHoursDayInput(
                            DayOfWeek: day.DayOfWeek,
                            IsClosed: day.IsClosed,
                            IsOpen24Hours: day.IsOpen24Hours,
                            Periods: day.Periods
                                .Select(period =>
                                    new BusinessHoursPeriodInput(
                                        period.OpensAt,
                                        period.ClosesAt))
                                .ToList()))
                    .ToList());

            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Store was not found."
                });
            }

            return Ok(result);
        }


        private static DeliveryOptionInput ToDeliveryOptionInput(
            SaveDeliveryOptionRequest request)
        {
            return new DeliveryOptionInput(
                request.Name,
                request.CoverageDescription,
                request.EstimatedTimeDescription,
                request.FeeType,
                request.FeeAmount,
                request.CurrencyCode,
                request.Conditions,
                request.IsActive);
        }

        [Authorize]
        [HttpGet("{storeId:guid}/delivery-options")]
        public async Task<ActionResult<IReadOnlyList<DeliveryOptionDto>>>
            GetDeliveryOptions(
                Guid storeId,
                CancellationToken ct)
        {
            var result = await _mediator.Send(
                new GetDeliveryOptionsQuery(
                    storeId,
                    HttpContext.GetUserId()),
                ct);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{storeId:guid}/delivery-options")]
        public async Task<ActionResult<DeliveryOptionDto>>
            CreateDeliveryOption(
                Guid storeId,
                [FromBody] SaveDeliveryOptionRequest request,
                CancellationToken ct)
        {
            var result = await _mediator.Send(
                new CreateDeliveryOptionCommand(
                    storeId,
                    HttpContext.GetUserId(),
                    ToDeliveryOptionInput(request)),
                ct);

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }

        [Authorize]
        [HttpPut("{storeId:guid}/delivery-options/{optionId:guid}")]
        public async Task<ActionResult<DeliveryOptionDto>>
            UpdateDeliveryOption(
                Guid storeId,
                Guid optionId,
                [FromBody] SaveDeliveryOptionRequest request,
                CancellationToken ct)
        {
            var result = await _mediator.Send(
                new UpdateDeliveryOptionCommand(
                    storeId,
                    HttpContext.GetUserId(),
                    optionId,
                    request.ExpectedVersion ?? 0,
                    ToDeliveryOptionInput(request)),
                ct);

            return Ok(result);
        }

        [Authorize]
        [HttpPatch("{storeId:guid}/delivery-options/{optionId:guid}/status")]
        public async Task<ActionResult<DeliveryOptionDto>>
            SetDeliveryOptionStatus(
                Guid storeId,
                Guid optionId,
                [FromBody] SetDeliveryOptionStatusRequest request,
                CancellationToken ct)
        {
            var result = await _mediator.Send(
                new SetDeliveryOptionStatusCommand(
                    storeId,
                    HttpContext.GetUserId(),
                    optionId,
                    request.ExpectedVersion,
                    request.IsActive),
                ct);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{storeId:guid}/pickup-delivery")]
        public async Task<ActionResult<FulfillmentDto>>
            SaveFulfillment(
                Guid storeId,
                [FromBody] SaveFulfillmentRequest request,
                CancellationToken ct)
        {
            var result = await _mediator.Send(
                new SaveFulfillmentCommand(
                    storeId,
                    HttpContext.GetUserId(),
                    request.IsPickupAvailable,
                    request.IsDeliveryAvailable,
                    request.FulfillmentNotes),
                ct);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{storeId:guid}/publish")]
        [ProducesResponseType(typeof(StorePublicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<StorePublicationDto>> Publish(
            Guid storeId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new PublishStoreCommand(storeId, HttpContext.GetUserId()),
                cancellationToken);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{storeId:guid}/accepting-orders")]
        [ProducesResponseType(typeof(StorePublicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<StorePublicationDto>> SetAcceptingOrders(
            Guid storeId,
            [FromBody] SetAcceptingOrdersRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new SetAcceptingOrdersCommand(
                    storeId,
                    HttpContext.GetUserId(),
                    request.IsAcceptingOrders!.Value),
                cancellationToken);

            return Ok(result);
        }

        private static bool TryParseBrandingKind(
            string value,
            out StoreBrandingImageKind kind)
        {
            return Enum.TryParse(
                       value,
                       ignoreCase: true,
                       out kind)
                   && Enum.IsDefined(kind);
        }


        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new { id });
        }
    }
}


