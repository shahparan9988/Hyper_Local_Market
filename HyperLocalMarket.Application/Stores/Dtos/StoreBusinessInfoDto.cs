using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Dtos
{
    public sealed record StoreBusinessInfoDto(
        Guid Id,
        string Name,
        string Slug,
        string? Description);
}
