using HyperLocalMarket.Api.Common;
using HyperLocalMarket.Api.Contracts.Stores;
using HyperLocalMarket.Application.Stores.Commands.CreateStore;
using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HyperLocalMarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class StoreController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoreController (IMediator mediator)
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

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new { id });
        }
    }
}
