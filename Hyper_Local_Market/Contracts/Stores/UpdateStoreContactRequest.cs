namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record UpdateStoreContactRequest(
        string? PhoneNumber,
        string? Email,
        string? TimeZoneId);
}
