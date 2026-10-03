using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Products.Dtos;
using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.SaveProduct
{
    public sealed class SaveProductCommandHandler(IProductCatalogRepository repository,
        IProductSlugService slugs, IUnitOfWork unitOfWork, TimeProvider time)
        : IRequestHandler<SaveProductCommand, SellerProductDto>
    {
        public async Task<SellerProductDto> Handle(SaveProductCommand request, CancellationToken ct)
        {
            await using var transaction = await repository.BeginOwnerWriteAsync(request.StoreId, request.UserId, ct);
            var input = request.Input;
            var now = time.GetUtcNow().UtcDateTime;
            var hash = CatalogRules.Hash(input);
            const string operation = "CreateProduct";
            if (!request.ProductId.HasValue)
            {
                if (request.IdempotencyKey is null || request.IdempotencyKey == Guid.Empty) throw new DomainException("Idempotency-Key is required.");
                var receipt = await repository.ReceiptAsync(request.StoreId, request.UserId, operation, request.IdempotencyKey.Value, ct);
                if (receipt is not null) return CatalogRules.Replay<SellerProductDto>(receipt, hash);
                if (input.ExpectedVersion.HasValue) throw new DomainException("New products must have no expected version.");
            }
            var product = request.ProductId.HasValue
                ? await repository.GetTrackedAsync(request.StoreId, request.ProductId.Value, ct) ?? throw new NotFoundException("Product was not found.")
                : Product.Create(request.StoreId, input.Name, await slugs.GenerateUniqueSlugAsync(request.StoreId, input.Name, ct), input.Description, null, now);
            if (request.ProductId.HasValue) CatalogRules.Version(product.Version, input.ExpectedVersion);
            if (product.Status == ProductStatus.Archived) throw new DomainException("Restore the archived product before editing.");
            if (await repository.NameExistsAsync(request.StoreId, input.Name.Trim(), request.ProductId, ct)) throw new DomainException("This store already has a product with that name.");
            var currency = transaction.Store.Location.CountryCode == "AU" ? "AUD" : "BDT";
            if (input.CurrencyCode != currency) throw new DomainException("Use the currency of the store location.");
            if (product.Variants.Any(x => x.Price.CurrencyCode != input.CurrencyCode)) throw new DomainException("Existing variant currency cannot be changed through this editor.");
            var categories = await repository.CategoriesAsync(request.StoreId, ct);
            if (input.StoreCategoryId.HasValue && !categories.Any(x => x.Id == input.StoreCategoryId)) throw new DomainException("The store category does not belong to this store.");
            if (input.MarketplaceCategoryId.HasValue)
            {
                var category = await repository.MarketplaceCategoryAsync(input.MarketplaceCategoryId.Value, ct);
                if (category is null || category.Status != CategoryStatus.Active) throw new DomainException("Choose an active HLM marketplace category.");
            }
            var type = input.Type == "Service" ? ProductType.Service : ProductType.Product;
            var availability = input.ManualAvailability == "Available" ? ProductAvailability.Available : ProductAvailability.Unavailable;
            if (type == ProductType.Service && (input.TrackInventory || input.IsPickupAvailable || input.DeliveryOptionIds.Count > 0)) throw new DomainException("Services do not use physical stock, pickup or delivery.");
            if (input.IsPickupAvailable && !transaction.Store.IsPickupAvailable) throw new DomainException("Pickup is not enabled for this store.");
            if (input.DeliveryOptionIds.Count > 0 && !transaction.Store.IsDeliveryAvailable) throw new DomainException("Delivery is not enabled for this store.");
            var delivery = await repository.LockDeliveryOptionsAsync(request.StoreId, input.DeliveryOptionIds, ct);
            if (delivery.Count != input.DeliveryOptionIds.Count || delivery.Any(x => !x.IsActive)) throw new DomainException("A delivery method is unavailable or belongs to another store.");
            var options = input.ToDomainOptions();
            var variantValues = input.ToDomainVariants();
            product.ValidateCatalogOptions(options, variantValues);
            if (await repository.HasForeignOptionIdsAsync(product.Id, options, ct))
                throw new DomainException("An option or value ID belongs to another product or option.");
            var ids = input.Variants.Select(x => x.Id).ToHashSet();
            if (ids.Count != input.Variants.Count || await repository.HasForeignVariantIdAsync(product.Id, ids, ct)) throw new DomainException("Variant IDs must be distinct and must not belong to another product.");
            var requestedSkus = input.Variants.Where(x => !string.IsNullOrWhiteSpace(x.Sku)).Select(x => x.Sku!.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (product.Variants.Any(x => !ids.Contains(x.Id) && x.Sku is not null && requestedSkus.Contains(x.Sku))) throw new DomainException("An SKU belongs to a retained historical variant. Reuse the existing variant or choose another SKU.");
            var stock = await repository.GetInventoryAsync(product.Variants.Select(x => x.Id).Concat(ids), ct);
            foreach (var old in product.Variants.Where(x => !ids.Contains(x.Id)))
                if (stock.TryGetValue(old.Id, out var inventory) && (inventory.ReservedQuantity > 0 || inventory.OnHandQuantity > 0) && old.IsListed)
                    throw new DomainException($"'{old.DisplayName}' still has stock. Keep it and uncheck Offer, or adjust its stock before removing the combination.");
            foreach (var variant in input.Variants)
            {
                stock.TryGetValue(variant.Id, out var inventory);
                if (!variant.IsOffered && inventory?.ReservedQuantity > 0)
                    throw new DomainException("Resolve reserved stock before turning off a variant's Offer checkbox.");
                if (input.TrackInventory)
                {
                    if (variant.Inventory is null) throw new DomainException("Stock details are required when tracking is enabled.");
                    CatalogRules.InventoryVersion(inventory, variant.Inventory.ExpectedVersion);
                    CatalogRules.Quantity(variant.Inventory.OnHandQuantity, inventory?.ReservedQuantity ?? 0);
                }
                else if (inventory?.ReservedQuantity > 0) throw new DomainException("Resolve reserved stock before disabling tracking.");
                if (input.Status == "Published" && variant.IsOffered && !variant.Price.HasValue) throw new DomainException("Set a selling price for every variant before publishing.");
            }
            var legacy = product.Images.Where(x => x.AssetId is null).ToDictionary(x => x.Id);
            var assets = await repository.LockImagesAsync(request.StoreId, request.UserId,
                input.ImageAssetIds.Where(id => !legacy.ContainsKey(id)).ToArray(), ct);
            var byId = assets.ToDictionary(x => x.Id);
            var images = input.ImageAssetIds.Select(id =>
            {
                if (legacy.TryGetValue(id, out var old)) return new CatalogImageValue(id, old.Url, true);
                if (!byId.TryGetValue(id, out var image) || image.Status != ImageProcessingStatus.Ready || image.PublicUrl is null)
                    throw new DomainException("An image is not ready or does not belong to this store.");
                return new CatalogImageValue(id, image.PublicUrl);
            }).ToList();
            if (request.ProductId.HasValue)
            {
                product.PrepareCatalogReplacement(ids, now);
                await unitOfWork.SaveChangesAsync(ct);
            }
            else repository.Add(product);
            product.ApplyCatalog(input.Name, input.Description, type, availability, input.CurrencyCode,
                input.StoreCategoryId, input.MarketplaceCategoryId, input.TrackInventory, input.IsPickupAvailable,
                input.VariantOptionName, variantValues, images, now, options);
            foreach (var variant in input.Variants)
            {
                stock.TryGetValue(variant.Id, out var inventory);
                if (input.TrackInventory)
                {
                    var quantity = variant.Inventory!.OnHandQuantity;
                    var previous = inventory?.OnHandQuantity ?? 0;
                    if (inventory is null)
                    {
                        inventory = InventoryItem.Create(variant.Id, true, false, quantity, 5, now);
                        repository.Add(inventory); stock.Add(variant.Id, inventory);
                    }
                    else
                    {
                        inventory.ChangePolicy(true, false, inventory.ReorderPoint, now);
                        inventory.AdjustOnHand(quantity, "ProductEdit", now);
                    }
                    if (previous != quantity) repository.Add(new InventoryAdjustment(request.StoreId, product.Id, variant.Id, request.UserId, previous, quantity, "ProductEdit", now));
                }
                else if (inventory is not null) inventory.ChangePolicy(false, false, inventory.ReorderPoint, now);
            }
            // Retire listings, not inventory history. Removed variants cannot take new stock reservations.
            foreach (var old in stock.Values.Where(x => !ids.Contains(x.ProductVariantId)))
                old.ChangePolicy(false, false, old.ReorderPoint, now);
            product.ReplaceDeliveryOptions(input.DeliveryOptionIds, now);
            if (input.Status == "Published") product.Publish(now); else product.MoveToDraft(now);
            await unitOfWork.SaveChangesAsync(ct);
            var result = SellerProductDto.From(product, stock);
            if (!request.ProductId.HasValue)
            {
                repository.AddReceipt(request.StoreId, request.UserId, operation, request.IdempotencyKey!.Value,
                    hash, JsonSerializer.Serialize(result, CatalogRules.Json), now);
                await unitOfWork.SaveChangesAsync(ct);
            }
            await transaction.CommitAsync(ct);
            return result;
        }
    }

}
