using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.ValueObjects
{
    public sealed class StoreLocation
    {
        // ISO-3166 alpha-2 recommended ("BD", "AU")
        public string CountryCode { get; }

        public GeoLocation Coordinates { get; }
        public PostalAddress Address { get; }
        public AdminArea AdminArea { get; }

        private StoreLocation() { } // EF Core

        public StoreLocation(
            string countryCode,
            GeoLocation coordinates,
            PostalAddress address,
            AdminArea adminArea)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("CountryCode is required.", nameof(countryCode));

            CountryCode = countryCode.Trim().ToUpperInvariant();

            Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            AdminArea = adminArea ?? throw new ArgumentNullException(nameof(adminArea));
        }
    }
}
