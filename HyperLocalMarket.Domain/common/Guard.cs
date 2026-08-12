using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.common
{
    internal static class Guard
    {
        public static string RequiredText(
            string? value,
            string parameterName,
            int maximumLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new DomainException($"{parameterName} is required.");
            }

            var normalized = value.Trim();

            if (normalized.Length > maximumLength)
            {
                throw new DomainException(
                    $"{parameterName} cannot exceed {maximumLength} characters.");
            }

            return normalized;
        }

        public static string? OptionalText(
            string? value,
            string parameterName,
            int maximumLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var normalized = value.Trim();

            if (normalized.Length > maximumLength)
            {
                throw new DomainException(
                    $"{parameterName} cannot exceed {maximumLength} characters.");
            }

            return normalized;
        }

        public static void NotEmpty(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException($"{parameterName} cannot be empty.");
            }
        }

        public static DateTime Utc(DateTime value, string parameterName)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                throw new DomainException($"{parameterName} must be a UTC value.");
            }

            return value;
        }

    }
}
