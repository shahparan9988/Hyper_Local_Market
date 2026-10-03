using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Stores;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateBusinessHours
{
    public sealed class UpdateBusinessHoursCommandHandler
    : IRequestHandler<
        UpdateBusinessHoursCommand,
        StoreBusinessHoursDto?>
    {
        private static readonly DayOfWeek[] OrderedDays =
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        ];

        private readonly IStoreRepository _storeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBusinessHoursCommandHandler(
            IStoreRepository storeRepository,
            IUnitOfWork unitOfWork)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreBusinessHoursDto?> Handle(
            UpdateBusinessHoursCommand request,
            CancellationToken cancellationToken)
        {
            var store =
                await _storeRepository.GetByIdAndUserIdAsTrackingAsync(
                    request.StoreId,
                    request.UserId,
                    cancellationToken);

            if (store is null)
            {
                return null;
            }

            var businessHours =
                new List<StoreBusinessHour>();

            foreach (var day in request.Hours)
            {
                var dayOfWeek = Enum.Parse<DayOfWeek>(
                    day.DayOfWeek,
                    ignoreCase: true);

                if (day.IsClosed)
                {
                    // A closed day has no database row.
                    continue;
                }

                if (day.IsOpen24Hours)
                {
                    businessHours.Add(
                        StoreBusinessHour.Open24Hours(dayOfWeek));

                    continue;
                }

                foreach (var period in day.Periods)
                {
                    var opensAt = TimeOnly.ParseExact(
                        period.OpensAt,
                        "HH:mm",
                        CultureInfo.InvariantCulture);

                    var closesAt = TimeOnly.ParseExact(
                        period.ClosesAt,
                        "HH:mm",
                        CultureInfo.InvariantCulture);

                    businessHours.Add(
                        StoreBusinessHour.Open(
                            dayOfWeek,
                            opensAt,
                            closesAt));
                }
            }

            store.SetBusinessHours(businessHours);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return CreateResult(store);
        }

        private static StoreBusinessHoursDto CreateResult(
            Store store)
        {
            var days = OrderedDays
                .Select(dayOfWeek =>
                {
                    var periodsForDay = store.BusinessHours
                        .Where(hour =>
                            hour.DayOfWeek == dayOfWeek)
                        .OrderBy(hour => hour.OpensAt)
                        .ToList();

                    var isOpen24Hours =
                        periodsForDay.Any(hour =>
                            hour.IsOpen24Hours);

                    var periods = periodsForDay
                        .Where(hour =>
                            !hour.IsOpen24Hours)
                        .Select(hour =>
                            new StoreBusinessHoursPeriodDto(
                                hour.OpensAt!.Value.ToString("HH:mm"),
                                hour.ClosesAt!.Value.ToString("HH:mm")))
                        .ToList();

                    return new StoreBusinessHoursDayDto(
                        DayOfWeek: dayOfWeek.ToString(),
                        IsClosed: periodsForDay.Count == 0,
                        IsOpen24Hours: isOpen24Hours,
                        Periods: periods);
                })
                .ToList();

            return new StoreBusinessHoursDto(
                store.Id,
                days);
        }
    }
}
