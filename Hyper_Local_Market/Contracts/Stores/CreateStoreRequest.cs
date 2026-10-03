namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record CreateStoreRequest
    (
        string Name,
        string? Description,
        string? PhoneNumber,
        string? Email,
        string TimeZoneId,
        StoreLocationRequest Location
    );
}
