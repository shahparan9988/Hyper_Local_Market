using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Dtos
{
    public sealed record StoreBusinessHoursDto(
        Guid Id,
        IReadOnlyList<StoreBusinessHoursDayDto> BusinessHours);

    public sealed record StoreBusinessHoursDayDto(
        string DayOfWeek,
        bool IsClosed,
        bool IsOpen24Hours,
        IReadOnlyList<StoreBusinessHoursPeriodDto> Periods);

    public sealed record StoreBusinessHoursPeriodDto(
        string OpensAt,
        string ClosesAt);
}
