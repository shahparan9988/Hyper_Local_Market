using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Stores
{
    public sealed class StoreBusinessHour
    {
        public Guid Id { get; private set; }

        public DayOfWeek DayOfWeek { get; private set; }

        public TimeOnly? OpensAt { get; private set; }
        public TimeOnly? ClosesAt { get; private set; }

        public bool IsOpen24Hours { get; private set; }

        private StoreBusinessHour()
        {
        }

        private StoreBusinessHour(
            DayOfWeek dayOfWeek,
            TimeOnly? opensAt,
            TimeOnly? closesAt,
            bool isOpen24Hours)
        {
            Id = Guid.NewGuid();
            DayOfWeek = dayOfWeek;
            OpensAt = opensAt;
            ClosesAt = closesAt;
            IsOpen24Hours = isOpen24Hours;
        }

        public static StoreBusinessHour Open(
            DayOfWeek dayOfWeek,
            TimeOnly opensAt,
            TimeOnly closesAt)
        {
            ValidateDayOfWeek(dayOfWeek);

            if (opensAt >= closesAt)
            {
                throw new ArgumentException(
                    "Closing time must be later than opening time. " +
                    "An overnight period must be split across two days.");
            }

            return new StoreBusinessHour(
                dayOfWeek,
                opensAt,
                closesAt,
                isOpen24Hours: false);
        }

        public static StoreBusinessHour Open24Hours(
            DayOfWeek dayOfWeek)
        {
            ValidateDayOfWeek(dayOfWeek);

            return new StoreBusinessHour(
                dayOfWeek,
                opensAt: null,
                closesAt: null,
                isOpen24Hours: true);
        }

        public bool Contains(TimeOnly localTime)
        {
            if (IsOpen24Hours)
            {
                return true;
            }

            return localTime >= OpensAt!.Value &&
                   localTime < ClosesAt!.Value;
        }

        private static void ValidateDayOfWeek(
            DayOfWeek dayOfWeek)
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dayOfWeek),
                    "Invalid day of the week.");
            }
        }
    }
}
