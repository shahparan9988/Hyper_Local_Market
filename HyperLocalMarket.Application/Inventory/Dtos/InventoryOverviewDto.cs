using HyperLocalMarket.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Inventory.Dtos
{
    public sealed record InventoryOverviewDto(
        PagedResult<InventoryRowDto> Results,
        InventorySummaryDto Summary);

    public sealed record InventorySummaryDto(
        int All,
        int InStock,
        int LowStock,
        int OutOfStock,
        int BackorderAvailable,
        int TrackingOff,
        int NeedsReview);

    public sealed record InventoryRowDto(
        Guid ProductId,
        Guid ProductVariantId,
        Guid? InventoryItemId,
        string ProductName,
        string VariantName,
        string? Sku,
        Guid? StoreCategoryId,
        string? StoreCategoryName,
        string? ImageUrl,
        string ProductStatus,
        bool IsVariantListed,
        bool IsVariantActive,
        bool TrackInventory,
        string StockStatus,
        decimal? OnHandQuantity,
        decimal? ReservedQuantity,
        decimal? AvailableQuantity,
        decimal? ReorderPoint,
        bool? AllowBackorder,
        int ProductVersion,
        int? InventoryVersion);
}
