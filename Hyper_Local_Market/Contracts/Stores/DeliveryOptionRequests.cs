namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record SaveDeliveryOptionRequest(
        string? Name,
        string? CoverageDescription,
        string? EstimatedTimeDescription,
        string? FeeType,
        decimal? FeeAmount,
        string? CurrencyCode,
        string? Conditions,
        bool IsActive,
        int? ExpectedVersion);
    public sealed record SetDeliveryOptionStatusRequest(
        bool IsActive,
        int ExpectedVersion);
    public sealed record SaveFulfillmentRequest(
        bool IsPickupAvailable,
        bool IsDeliveryAvailable,
        string? FulfillmentNotes);
    public sealed record SaveProductDeliverySelectionRequest(
        int ExpectedVersion,
        IReadOnlyList<Guid>? DeliveryOptionIds);

}
