using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetMyStores
{
    public sealed record MyStoreDto(
        Guid Id,
        string Name,
        string Status,
        MyStoreLocationDto Location);

    public sealed record MyStoreLocationDto(
        MyStoreAddressDto Address);

    public sealed record MyStoreAddressDto(
        string? AddressLine1,
        string? Locality,
        string? Region);
}
