using HyperLocalMarket.Application.Common.PhoneNumbers;
using HyperLocalMarket.Application.Common.TimeZones;
using HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Services
{
    public static class StorePublicationRules
    {
        public static IReadOnlyDictionary<string, string[]> GetErrors(Store store)
        {
            return Validate(
                store.Name,
                store.PhoneNumber,
                store.TimeZoneId,
                store.Location?.CountryCode,
                store.Location?.Address?.AddressLine1,
                store.Location?.Coordinates?.Latitude,
                store.Location?.Coordinates?.Longitude);
        }

        public static IReadOnlyDictionary<string, string[]> GetErrors(
            StoreSetupStatusData store)
        {
            return Validate(
                store.Name,
                store.PhoneNumber,
                store.TimeZoneId,
                store.Location?.CountryCode,
                store.Location?.Address?.AddressLine1,
                store.Location?.Coordinates?.Latitude,
                store.Location?.Coordinates?.Longitude);
        }

        public static void EnsureReady(Store store)
        {
            var errors = GetErrors(store);

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }

        public static bool CanAcceptOrders(Store store)
        {
            // Eligibility to enable orders; the seller's current switch is separate.
            return store.Status == StoreStatus.Active && GetErrors(store).Count == 0;
        }

        private static IReadOnlyDictionary<string, string[]> Validate(
            string? name,
            string? phoneNumber,
            string? timeZoneId,
            string? countryCode,
            string? addressLine1,
            double? latitude,
            double? longitude)
        {
            var errors = new Dictionary<string, string[]>();
            var country = countryCode?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors["name"] = ["Add your store name in Business info."];
            }

            if (string.IsNullOrWhiteSpace(country) ||
                country.Length != 2 ||
                country.Any(character => character is < 'A' or > 'Z'))
            {
                errors["countryCode"] = ["Choose a valid country in Location."];
            }

            if (string.IsNullOrWhiteSpace(addressLine1))
            {
                errors["addressLine1"] = ["Add your street address in Location."];
            }

            // Zero is a valid coordinate. Missing, non-finite and out-of-range values are not.
            if (!latitude.HasValue || !double.IsFinite(latitude.Value) ||
                latitude.Value is < -90 or > 90)
            {
                errors["latitude"] = ["Confirm a valid map location (latitude)."];
            }

            if (!longitude.HasValue || !double.IsFinite(longitude.Value) ||
                longitude.Value is < -180 or > 180)
            {
                errors["longitude"] = ["Confirm a valid map location (longitude)."];
            }

            if (!SupportedPhoneNumber.TryNormalize(phoneNumber, out _))
            {
                errors["phoneNumber"] =
                    ["Add a valid Bangladesh or Australian phone number in Contact."];
            }

            if (!SupportedStoreTimeZones.IsSupported(timeZoneId))
            {
                errors["timeZoneId"] = ["Choose a supported timezone in Contact."];
            }
            else if (!string.IsNullOrWhiteSpace(country) &&
                     !SupportedStoreTimeZones.IsAllowedForCountry(country, timeZoneId!))
            {
                errors["timeZoneId"] =
                    ["Choose a timezone that matches your store's country."];
            }

            return errors;
        }
    }

}
