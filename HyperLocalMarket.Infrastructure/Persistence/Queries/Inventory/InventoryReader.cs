using HyperLocalMarket.Application.Inventory.Dtos;
using HyperLocalMarket.Application.Inventory.Enums;
using HyperLocalMarket.Application.Inventory.Queries;
using HyperLocalMarket.Application.Products.Services;
using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Shared.Exceptions;
using HyperLocalMarket.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Queries.Inventory
{
    public sealed class InventoryReader(AppDbContext db) : IInventoryReader
    {
        public async Task<InventoryOverviewDto> GetOverviewAsync(
            Guid storeId,
            Guid userId,
            InventoryOverviewFilter filter,
            CancellationToken cancellationToken)
        {
            var ownsStore = await db.Stores
                .AsNoTracking()
                .AnyAsync(
                    store => store.Id == storeId && store.UserId == userId,
                    cancellationToken);

            if (!ownsStore)
            {
                throw new NotFoundException("Store was not found.");
            }

            var categoryIds = await GetCategoryIdsAsync(
                storeId, filter.CategoryId, cancellationToken);

            // One row per variant. LEFT JOIN keeps untracked variants
            // and tracked variants with missing inventory visible.
            var query =
                from product in db.Products.AsNoTracking()
                join variant in db.ProductVariants.AsNoTracking()
                    on product.Id equals variant.ProductId
                join inventory in db.InventoryItems.AsNoTracking()
                    on variant.Id equals inventory.ProductVariantId
                    into inventoryGroup
                from inventory in inventoryGroup.DefaultIfEmpty()
                join category in db.StoreProductCategories.AsNoTracking()
                        .Where(category => category.StoreId == storeId)
                    on product.StoreCategoryId equals (Guid?)category.Id
                    into categoryGroup
                from category in categoryGroup.DefaultIfEmpty()
                where product.StoreId == storeId
                    && product.Type == ProductType.Product
                    && (variant.IsListed ||
                        (inventory != null &&
                            (inventory.OnHandQuantity != 0 ||
                             inventory.ReservedQuantity != 0)))
                let isTracked = product.TrackInventory
                    && inventory != null && inventory.TrackInventory
                let needsReview =
                    (product.TrackInventory && inventory == null) ||
                    (inventory != null &&
                        product.TrackInventory != inventory.TrackInventory)
                let available = inventory == null
                    ? 0m
                    : inventory.OnHandQuantity - inventory.ReservedQuantity
                let stockStatus = needsReview
                    ? InventoryStockStatus.NeedsReview
                    : !product.TrackInventory
                        ? InventoryStockStatus.TrackingOff
                        : available <= 0
                            ? inventory!.AllowBackorder
                                ? InventoryStockStatus.BackorderAvailable
                                : InventoryStockStatus.OutOfStock
                            : available <= inventory!.ReorderPoint
                                ? InventoryStockStatus.LowStock
                                : InventoryStockStatus.InStock
                select new
                {
                    ProductId = product.Id,
                    ProductVariantId = variant.Id,
                    InventoryItemId = inventory == null
                        ? (Guid?)null : inventory.Id,
                    ProductName = product.Name,
                    VariantName = variant.DisplayName,
                    variant.Sku,
                    product.StoreCategoryId,
                    StoreCategoryName = category == null ? null : category.Name,
                    ProductStatus = product.Status,
                    IsVariantListed = variant.IsListed,
                    IsVariantActive = variant.Status == ProductVariantStatus.Active,
                    product.TrackInventory,
                    StockStatus = stockStatus,
                    OnHandQuantity = isTracked
                        ? (decimal?)inventory!.OnHandQuantity : null,
                    ReservedQuantity = isTracked
                        ? (decimal?)inventory!.ReservedQuantity : null,
                    AvailableQuantity = isTracked ? (decimal?)available : null,
                    ReorderPoint = isTracked
                        ? (decimal?)inventory!.ReorderPoint : null,
                    AllowBackorder = isTracked
                        ? (bool?)inventory!.AllowBackorder : null,
                    ProductVersion = product.Version,
                    InventoryVersion = inventory == null
                        ? (int?)null : inventory.Version,
                    variant.CatalogOrder
                };

            if (!filter.IncludeArchived)
            {
                query = query.Where(row =>
                    row.ProductStatus != ProductStatus.Archived);
            }

            if (filter.TrackInventory.HasValue)
            {
                query = query.Where(row =>
                    row.TrackInventory == filter.TrackInventory.Value);
            }

            if (categoryIds is not null)
            {
                query = query.Where(row => row.StoreCategoryId.HasValue &&
                    categoryIds.Contains(row.StoreCategoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var pattern = "%" + EscapeLikePattern(filter.Search.Trim()) + "%";

                query = query.Where(row =>
                    EF.Functions.ILike(row.ProductName, pattern, "\\") ||
                    EF.Functions.ILike(row.VariantName, pattern, "\\") ||
                    (row.Sku != null &&
                        EF.Functions.ILike(row.Sku, pattern, "\\")));
            }

            // Tab counts use all the filters above, before StockStatus.
            // The same status expression powers the counts and page filter.
            var counts = await query
                .GroupBy(row => row.StockStatus)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToDictionaryAsync(
                    value => value.Status,
                    value => value.Count,
                    cancellationToken);

            var summary = new InventorySummaryDto(
                All: counts.Values.Sum(),
                InStock: counts.GetValueOrDefault(InventoryStockStatus.InStock),
                LowStock: counts.GetValueOrDefault(InventoryStockStatus.LowStock),
                OutOfStock: counts.GetValueOrDefault(InventoryStockStatus.OutOfStock),
                BackorderAvailable: counts.GetValueOrDefault(InventoryStockStatus.BackorderAvailable),
                TrackingOff: counts.GetValueOrDefault(InventoryStockStatus.TrackingOff),
                NeedsReview: counts.GetValueOrDefault(InventoryStockStatus.NeedsReview));

            var totalCount = summary.All;

            if (filter.StockStatus.HasValue)
            {
                var selectedStatus = filter.StockStatus.Value;
                query = query.Where(row => row.StockStatus == selectedStatus);
                totalCount = counts.GetValueOrDefault(selectedStatus);
            }

            var pageRows = await query
                .OrderBy(row => row.ProductName)
                .ThenBy(row => row.ProductId)
                .ThenBy(row => row.CatalogOrder)
                .ThenBy(row => row.ProductVariantId)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(row => new
                {
                    Row = row,
                    ImageUrl = db.ProductImages
                        .Where(image => image.ProductId == row.ProductId &&
                            (image.ProductVariantId == null ||
                             image.ProductVariantId == row.ProductVariantId))
                        .OrderByDescending(image =>
                            image.ProductVariantId == row.ProductVariantId)
                        .ThenByDescending(image => image.IsPrimary)
                        .ThenBy(image => image.DisplayOrder)
                        .ThenBy(image => image.Id)
                        .Select(image => image.Url)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var items = pageRows.Select(entry =>
            {
                var row = entry.Row;

                return new InventoryRowDto(
                    row.ProductId,
                    row.ProductVariantId,
                    row.InventoryItemId,
                    row.ProductName,
                    row.VariantName,
                    row.Sku,
                    row.StoreCategoryId,
                    row.StoreCategoryName,
                    entry.ImageUrl,
                    row.ProductStatus == ProductStatus.Active
                        ? "Published" : row.ProductStatus.ToString(),
                    row.IsVariantListed,
                    row.IsVariantActive,
                    row.TrackInventory,
                    row.StockStatus.ToString(),
                    row.OnHandQuantity,
                    row.ReservedQuantity,
                    row.AvailableQuantity,
                    row.ReorderPoint,
                    row.AllowBackorder,
                    row.ProductVersion,
                    row.InventoryVersion);
            }).ToList();

            var pagedResult =
                PagedResult<InventoryRowDto>.Create(
                    items,
                    filter.Page,
                    filter.PageSize,
                    totalCount);

            return new InventoryOverviewDto(
                pagedResult, summary);
        }

        private async Task<Guid[]?> GetCategoryIdsAsync(
            Guid storeId,
            Guid? categoryId,
            CancellationToken cancellationToken)
        {
            if (!categoryId.HasValue)
            {
                return null;
            }

            var categories = await db.StoreProductCategories
                .AsNoTracking()
                .Where(category => category.StoreId == storeId)
                .Select(category => new { category.Id, category.ParentId })
                .ToListAsync(cancellationToken);

            if (!categories.Any(category => category.Id == categoryId.Value))
            {
                throw new DomainException("The category does not belong to this store.");
            }

            return CatalogRules.Descendants(
                categories.Select(category => (category.Id, category.ParentId)),
                categoryId.Value).ToArray();
        }

        private static string EscapeLikePattern(string value)
        {
            return value.Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");
        }
    }
}
