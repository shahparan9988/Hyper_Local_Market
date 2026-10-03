using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SetProductAvailability
{
    public sealed class SetProductAvailabilityCommandHandler(
        IProductCatalogRepository repository,
        IUnitOfWork unitOfWork,
        TimeProvider time)
        : IRequestHandler<SetProductAvailabilityCommand, SellerProductDto>
    {
        public async Task<SellerProductDto> Handle(
            SetProductAvailabilityCommand request, CancellationToken ct)
        {
            await using var transaction = 
                await repository.BeginOwnerWriteAsync(request.StoreId, request.UserId, ct);

            var product = await repository.GetTrackedAsync(
                request.StoreId,
                request.ProductId,
                ct) ?? throw new NotFoundException("Product was not found.");

            CatalogRules.Version(product.Version, request.ExpectedVersion);

            var availability = request.Availability
                switch {
                    "Available" => ProductAvailability.Available,
                    "Unavailable" => ProductAvailability.Unavailable,
                    _ => throw new DomainException("Invalid availability.") };

            product.SetManualAvailability(availability, time.GetUtcNow().UtcDateTime);

            await unitOfWork.SaveChangesAsync(ct);

            var result = SellerProductDto.From(
                product, await repository.GetInventoryAsync(product.Variants.Select(x => x.Id), ct));

            await transaction.CommitAsync(ct); return result;
        }
    }

}
