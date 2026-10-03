using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Common.PhoneNumbers
{
    public static class SupportedPhoneNumber
    {
        private static readonly PhoneNumberUtil PhoneUtility =
            PhoneNumberUtil.GetInstance();

        private static readonly HashSet<string> SupportedRegions =
            new(StringComparer.OrdinalIgnoreCase)
            {
            "BD",
            "AU"
            };

        public static bool TryNormalize(
            string? input,
            out string normalizedPhoneNumber)
        {
            normalizedPhoneNumber = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            try
            {
                // Because the frontend sends international E.164,
                // no default region is required.
                var parsedPhoneNumber =
                    PhoneUtility.Parse(
                        input.Trim(),
                        defaultRegion: null);

                var regionCode =
                    PhoneUtility.GetRegionCodeForNumber(
                        parsedPhoneNumber);

                if (string.IsNullOrWhiteSpace(regionCode))
                {
                    return false;
                }

                if (!SupportedRegions.Contains(regionCode))
                {
                    return false;
                }

                if (!PhoneUtility.IsValidNumberForRegion(
                        parsedPhoneNumber,
                        regionCode))
                {
                    return false;
                }

                normalizedPhoneNumber =
                    PhoneUtility.Format(
                        parsedPhoneNumber,
                        PhoneNumberFormat.E164);

                return true;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }

        public static string Normalize(string input)
        {
            if (!TryNormalize(
                    input,
                    out var normalizedPhoneNumber))
            {
                throw new ArgumentException(
                    "The phone number is invalid.",
                    nameof(input));
            }

            return normalizedPhoneNumber;
        }
    }
}
