using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SetProductStatus
{
    public sealed class SetProductStatusCommandHandler(IProductCatalogRepository repository, IUnitOfWork unitOfWork, TimeProvider time)
        : IRequestHandler<SetProductStatusCommand, SellerProductDto>
    {
        public async Task<SellerProductDto> Handle(SetProductStatusCommand request, CancellationToken ct)
        {
            await using var transaction = await repository.BeginOwnerWriteAsync(request.StoreId, request.UserId, ct);
            var product = await repository.GetTrackedAsync(request.StoreId, request.ProductId, ct) ?? throw new NotFoundException("Product was not found.");
            CatalogRules.Version(product.Version, request.ExpectedVersion);
            if (request.Status == "Archived") product.Archive(time.GetUtcNow().UtcDateTime);
            else if (request.Status == "Draft") product.RestoreCatalogDraft(time.GetUtcNow().UtcDateTime);
            else throw new DomainException("Choose Archived or Draft. Publish through the product editor.");
            await unitOfWork.SaveChangesAsync(ct);
            var result = SellerProductDto.From(product, await repository.GetInventoryAsync(product.Variants.Select(x => x.Id), ct));
            await transaction.CommitAsync(ct); return result;
        }
    }

}
