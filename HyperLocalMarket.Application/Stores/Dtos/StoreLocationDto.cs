using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Dtos
{
    public sealed record GeoLocationDto(double Latitude, double Longitude);
    public sealed record PostalAddressDto(
        string? AddressLine1,
        string? AddressLine2,
        string? Locality,
        string? Region,
        string? Postcode,
        string? Landmark
    );

    public sealed record AdminAreaDto(
        int? Level1Id,
        int? Level2Id,
        int? Level3Id,
        string? Level1,
        string? Level2,
        string? Level3,
        string? Level4
    );

    public sealed record StoreLocationDto(
        string CountryCode,
        GeoLocationDto Coordinates,
        PostalAddressDto Address,
        AdminAreaDto AdminArea
        );
    }

