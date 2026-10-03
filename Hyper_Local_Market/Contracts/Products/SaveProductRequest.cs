using HyperLocalMarket.Application.Products.Dtos;

namespace HyperLocalMarket.Api.Contracts.Products
{
    public sealed record SaveProductRequest(
        int? ExpectedVersion,
        string Name,
        string? Description,
        string Type,
        string Status,
        string CurrencyCode,
        Guid? StoreCategoryId,
        Guid? MarketplaceCategoryId,
        bool TrackInventory,
        string ManualAvailability,
        string? VariantOptionName,
        IReadOnlyList<ProductVariantInput> Variants,
        IReadOnlyList<Guid> ImageAssetIds,
        bool IsPickupAvailable,
        IReadOnlyList<Guid> DeliveryOptionIds,
        IReadOnlyList<ProductOptionInput>? Options)
    {
        public SaveProductInput ToInput() => new(
            ExpectedVersion,
            Name,
            Description,
            Type,
            Status,
            CurrencyCode,
            StoreCategoryId,
            MarketplaceCategoryId,
            TrackInventory,
            ManualAvailability,
            VariantOptionName,
            Variants,
            ImageAssetIds,
            IsPickupAvailable,
            DeliveryOptionIds,
            Options);
    }
}
