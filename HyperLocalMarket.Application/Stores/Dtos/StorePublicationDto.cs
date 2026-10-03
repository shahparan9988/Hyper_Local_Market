using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Dtos
{
    public sealed record StorePublicationDto(
        Guid Id,
        string Status,
        bool IsAcceptingOrders,
        bool CanAcceptOrders,
        DateTime? UpdatedAtUtc);
}
