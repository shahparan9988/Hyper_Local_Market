using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateLocation
{
    public sealed record UpdateLocationCommand(
        Guid StoreId,
        Guid UserId,
        string? CountryCode,
        double? Latitude,
        double? Longitude,
        string? AddressLine1,
        string? AddressLine2,
        string? Locality,
        string? Region,
        string? Postcode,
        string? Landmark,
        int? Level1Id,
        int? Level2Id,
        int? Level3Id,
        string? Level1,
        string? Level2,
        string? Level3,
        string? Level4)
        : IRequest<UpdateStoreLocationResultDto?>;
}
