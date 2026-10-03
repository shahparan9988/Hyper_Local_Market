using HyperLocalMarket.Application.Inventory.Enums;
using System.ComponentModel.DataAnnotations;

namespace HyperLocalMarket.Api.Contracts.Inventory
{
    public sealed class GetInventoryOverviewRequest
    {
        public string? Search { get; init; }
        public Guid? CategoryId { get; init; }
        public bool? TrackInventory { get; init; }
        public InventoryStockStatus? StockStatus { get; init; }
        public bool IncludeArchived { get; init; } = true;

        [Range(1, int.MaxValue)]
        public int Page { get; init; } = 1;

        [Range(1, 200)]
        public int PageSize { get; init; } = 20;
    }
}
