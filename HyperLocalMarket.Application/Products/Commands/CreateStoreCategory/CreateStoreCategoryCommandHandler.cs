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
using System.Text.Json;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.CreateStoreCategory
{
    public sealed class CreateStoreCategoryCommandHandler(IProductCatalogRepository repository, IUnitOfWork unitOfWork, TimeProvider time)
        : IRequestHandler<CreateStoreCategoryCommand, StoreCategoryDto>
    {
        public async Task<StoreCategoryDto> Handle(CreateStoreCategoryCommand request, CancellationToken ct)
        {
            if (request.IdempotencyKey == Guid.Empty) throw new DomainException("Idempotency-Key is required.");
            await using var transaction = await repository.BeginOwnerWriteAsync(request.StoreId, request.UserId, ct);
            var hash = CatalogRules.Hash(new { request.Name, request.ParentId });
            const string operation = "CreateStoreCategory";
            var receipt = await repository.ReceiptAsync(request.StoreId, request.UserId, operation, request.IdempotencyKey, ct);
            if (receipt is not null) return CatalogRules.Replay<StoreCategoryDto>(receipt, hash);
            var categories = await repository.CategoriesAsync(request.StoreId, ct);
            var byId = categories.ToDictionary(x => x.Id); var visited = new HashSet<Guid>();
            for (var id = request.ParentId; id.HasValue;)
            {
                if (!byId.TryGetValue(id.Value, out var parent)) throw new DomainException("The parent category does not belong to this store.");
                if (!visited.Add(id.Value)) throw new DomainException("Category parents contain a cycle.");
                id = parent.ParentId;
            }
            var category = new StoreProductCategory(request.StoreId, request.ParentId, request.Name, time.GetUtcNow().UtcDateTime);
            if (categories.Any(x => x.ParentId == category.ParentId && x.NormalizedName == category.NormalizedName)) throw new DomainException("This name already exists under that parent.");
            repository.Add(category);
            var result = StoreCategoryDto.From(category);
            repository.AddReceipt(request.StoreId, request.UserId, operation, request.IdempotencyKey, hash, JsonSerializer.Serialize(result, CatalogRules.Json), time.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
            return result;
        }
    }

}
