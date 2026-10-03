using HyperLocalMarket.Application.Stores.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateBusinessHours
{
    public sealed record UpdateBusinessHoursCommand(
        Guid StoreId,
        Guid UserId,
        IReadOnlyList<BusinessHoursDayInput> Hours)
        : IRequest<StoreBusinessHoursDto?>;

    public sealed record BusinessHoursDayInput(
        string DayOfWeek,
        bool IsClosed,
        bool IsOpen24Hours,
        IReadOnlyList<BusinessHoursPeriodInput> Periods);

    public sealed record BusinessHoursPeriodInput(
        string OpensAt,
        string ClosesAt);
}
