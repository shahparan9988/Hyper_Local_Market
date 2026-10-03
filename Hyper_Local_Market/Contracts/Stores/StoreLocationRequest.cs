namespace HyperLocalMarket.Api.Contracts.Stores
{
    public sealed record StoreLocationRequest(
         string CountryCode,
         GeoLocationRequest Coordinates,
         PostalAddressRequest Address,
         AdminAreaRequest AdminArea
    );

    public sealed record GeoLocationRequest(double Latitude, double Longitude);

    public sealed record PostalAddressRequest(
        string? AddressLine1,
        string? AddressLine2,
        string? Locality,
        string? Region,
        string? Postcode,
        string? Landmark
    );

    public sealed record AdminAreaRequest(
        int? Level1Id,
        int? Level2Id,
        int? Level3Id,
        string? Level1,
        string? Level2,
        string? Level3,
        string? Level4
    );
}
