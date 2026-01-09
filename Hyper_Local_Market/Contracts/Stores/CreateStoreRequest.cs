namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record CreateStoreRequest
    (
        string Name,
        string Street,
        string Suburb,
        string Postcode,
        string City,
        string State,
        string Country
    );
}
