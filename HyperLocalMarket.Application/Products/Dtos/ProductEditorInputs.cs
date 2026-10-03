using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Dtos
{
    public sealed record ProductOptionValueInput(Guid Id, string Value);
    public sealed record ProductOptionInput(Guid Id, string Name, IReadOnlyList<ProductOptionValueInput> Values);
    public sealed record ProductVariantSelectionInput(Guid OptionId, Guid ValueId);
    public sealed record ProductInventoryInput(int? ExpectedVersion, decimal OnHandQuantity);
    public sealed record ProductVariantInput(Guid Id, string Name, string? Sku, decimal? Price,
        decimal? CompareAtPrice, ProductInventoryInput? Inventory,
        bool IsOffered = true, IReadOnlyList<ProductVariantSelectionInput>? Selections = null);
    public sealed record SaveProductInput(int? ExpectedVersion, string Name, string? Description,
        string Type, string Status, string CurrencyCode, Guid? StoreCategoryId, Guid? MarketplaceCategoryId,
        bool TrackInventory, string ManualAvailability, string? VariantOptionName,
        IReadOnlyList<ProductVariantInput> Variants, IReadOnlyList<Guid> ImageAssetIds,
        bool IsPickupAvailable, IReadOnlyList<Guid> DeliveryOptionIds,
        IReadOnlyList<ProductOptionInput>? Options = null);
    public sealed record InventoryVariantInput(Guid Id, int? ExpectedVersion, decimal OnHandQuantity);
    public sealed record InventoryAdjustmentInput(int ExpectedVersion, string Reason, IReadOnlyList<InventoryVariantInput> Variants);
    public sealed record ProductListFilter(string Search = "", Guid? CategoryId = null,
        string Status = "Current", int Page = 1, int PageSize = 20);
}
