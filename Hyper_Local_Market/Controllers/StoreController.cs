using HyperLocalMarket.Api.Contracts.Stores;
using HyperLocalMarket.Application.Stores.Commands.CreateStore;
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
            var command = new CreateStoreCommand(
                request.Name,
                request.Street,
                request.Suburb,
                request.City,
                request.State,
                request.Postcode,
                request.Country
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
