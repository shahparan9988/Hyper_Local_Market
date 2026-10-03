using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Dtos
{
    public sealed record StoreContactDto(
        Guid Id,
        string PhoneNumber,
        string? Email,
        string TimeZoneId);
}
