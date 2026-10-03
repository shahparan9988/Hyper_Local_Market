using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.AdjustProductInventory
{
    public sealed class AdjustProductInventoryCommandHandler(IProductCatalogRepository repository, IUnitOfWork unitOfWork, TimeProvider time)
        : IRequestHandler<AdjustProductInventoryCommand, SellerProductDto>
    {
        public async Task<SellerProductDto> Handle(AdjustProductInventoryCommand request, CancellationToken ct)
        {
            await using var transaction = await repository.BeginOwnerWriteAsync(request.StoreId, request.UserId, ct);
            var p = await repository.GetTrackedAsync(request.StoreId, request.ProductId, ct) ?? throw new NotFoundException("Product was not found.");
            var input = request.Input ?? throw new DomainException("Stock details are required.");
            CatalogRules.Version(p.Version, input.ExpectedVersion);
            if (!p.TrackInventory || p.Type == ProductType.Service || p.Status == ProductStatus.Archived) throw new DomainException("This listing does not accept stock adjustments.");
            if (input.Reason is not ("CountCorrection" or "NewStock" or "ExternalSale" or "Damaged")) throw new DomainException("Choose an adjustment reason.");
            var ids = p.Variants.Where(x => x.IsListed).Select(x => x.Id).ToHashSet();
            if (input.Variants is null || input.Variants.Any(x => x is null) || input.Variants.Count != ids.Count || input.Variants.Select(x => x.Id).Distinct().Count() != ids.Count || input.Variants.Any(x => !ids.Contains(x.Id))) throw new DomainException("Reload the current variant list before adjusting stock.");
            var stock = await repository.GetInventoryAsync(ids, ct);
            foreach (var update in input.Variants)
            {
                if (!stock.TryGetValue(update.Id, out var item)) throw new ConflictException("Inventory changed. Reload the product.");
                CatalogRules.InventoryVersion(item, update.ExpectedVersion);
                CatalogRules.Quantity(update.OnHandQuantity, item.ReservedQuantity);
                var previous = item.OnHandQuantity;
                item.AdjustOnHand(update.OnHandQuantity, input.Reason, time.GetUtcNow().UtcDateTime);
                repository.Add(new InventoryAdjustment(request.StoreId, p.Id, update.Id, request.UserId, previous, update.OnHandQuantity, input.Reason, time.GetUtcNow().UtcDateTime));
            }
            p.Touch(time.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(ct);
            var result = SellerProductDto.From(p, stock);
            await transaction.CommitAsync(ct); return result;
        }
    }
}
