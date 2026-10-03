using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateBusinessHours
{
    public sealed class UpdateBusinessHoursCommandValidator
    : AbstractValidator<UpdateBusinessHoursCommand>
    {
        private static readonly DayOfWeek[] RequiredDays =
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        ];

        public UpdateBusinessHoursCommandValidator()
        {
            RuleFor(command => command.StoreId)
                .NotEmpty();

            RuleFor(command => command.UserId)
                .NotEmpty();

            RuleFor(command => command.Hours)
                .NotNull()
                .Custom(ValidateDays);
        }

        private static void ValidateDays(
            IReadOnlyList<BusinessHoursDayInput>? days,
            ValidationContext<UpdateBusinessHoursCommand> context)
        {
            if (days is null)
            {
                return;
            }

            if (days.Count != 7)
            {
                context.AddFailure(
                    "hours",
                    "Business hours must contain all seven days.");
            }

            var parsedDays = new HashSet<DayOfWeek>();

            for (var dayIndex = 0;
                 dayIndex < days.Count;
                 dayIndex++)
            {
                var day = days[dayIndex];
                var dayPath = $"hours[{dayIndex}]";

                if (!Enum.TryParse<DayOfWeek>(
                        day.DayOfWeek,
                        ignoreCase: true,
                        out var parsedDay) ||
                    !Enum.IsDefined(parsedDay))
                {
                    context.AddFailure(
                        $"{dayPath}.dayOfWeek",
                        $"'{day.DayOfWeek}' is not a valid day.");

                    continue;
                }

                if (!parsedDays.Add(parsedDay))
                {
                    context.AddFailure(
                        $"{dayPath}.dayOfWeek",
                        $"{parsedDay} appears more than once.");
                }

                if (day.IsClosed && day.IsOpen24Hours)
                {
                    context.AddFailure(
                        $"{dayPath}.isOpen24Hours",
                        $"{parsedDay} cannot be closed and open 24 hours.");
                }

                // Periods are ignored when the day is closed or open 24 hours.
                if (day.IsClosed || day.IsOpen24Hours)
                {
                    continue;
                }

                if (day.Periods is null ||
                    day.Periods.Count == 0)
                {
                    context.AddFailure(
                        $"{dayPath}.periods",
                        $"At least one opening period is required for {parsedDay}.");

                    continue;
                }

                if (day.Periods.Count > 4)
                {
                    context.AddFailure(
                        $"{dayPath}.periods",
                        $"A maximum of four periods is allowed for {parsedDay}.");
                }

                ValidatePeriods(
                    day,
                    dayIndex,
                    parsedDay,
                    context);
            }

            foreach (var requiredDay in RequiredDays)
            {
                if (!parsedDays.Contains(requiredDay))
                {
                    context.AddFailure(
                        "hours",
                        $"{requiredDay} is missing.");
                }
            }
        }

        private static void ValidatePeriods(
            BusinessHoursDayInput day,
            int dayIndex,
            DayOfWeek parsedDay,
            ValidationContext<UpdateBusinessHoursCommand> context)
        {
            var validPeriods =
                new List<(TimeOnly OpensAt, TimeOnly ClosesAt)>();

            for (var periodIndex = 0;
                 periodIndex < day.Periods.Count;
                 periodIndex++)
            {
                var period = day.Periods[periodIndex];

                var periodPath =
                    $"hours[{dayIndex}].periods[{periodIndex}]";

                var opensAtIsValid = TryParseTime(
                    period.OpensAt,
                    out var opensAt);

                var closesAtIsValid = TryParseTime(
                    period.ClosesAt,
                    out var closesAt);

                if (!opensAtIsValid)
                {
                    context.AddFailure(
                        $"{periodPath}.opensAt",
                        "Enter a valid opening time.");

                    continue;
                }

                if (!closesAtIsValid)
                {
                    context.AddFailure(
                        $"{periodPath}.closesAt",
                        "Enter a valid closing time.");

                    continue;
                }

                if (opensAt >= closesAt)
                {
                    context.AddFailure(
                        periodPath,
                        "Closing time must be later than opening time. " +
                        "Split overnight hours across two days.");

                    continue;
                }

                validPeriods.Add((opensAt, closesAt));
            }

            var orderedPeriods = validPeriods
                .OrderBy(period => period.OpensAt)
                .ToList();

            for (var index = 1;
                 index < orderedPeriods.Count;
                 index++)
            {
                var previous = orderedPeriods[index - 1];
                var current = orderedPeriods[index];

                if (current.OpensAt < previous.ClosesAt)
                {
                    context.AddFailure(
                        $"hours[{dayIndex}].periods",
                        $"Opening periods overlap on {parsedDay}.");

                    break;
                }
            }
        }

        private static bool TryParseTime(
            string? value,
            out TimeOnly time)
        {
            return TimeOnly.TryParseExact(
                value,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out time);
        }
    }
}
