using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.ValueObjects
{
    public sealed class PostalAddress
    {
        public string? AddressLine1 { get; }
        public string? AddressLine2 { get; }

        // Generic human-friendly locality (BD: Area, AU: Suburb)
        public string? Locality { get; }

        // Generic region (BD: Division, AU: State)
        public string? Region { get; }

        public string? Postcode { get; }
        public string? Landmark { get; }

        private PostalAddress() { }

        public PostalAddress(string? addressLine1, string? addressLine2, string? locality, string? region, string? postcode, string? landmark)
        {
            AddressLine1 = string.IsNullOrWhiteSpace(addressLine1) ? null : addressLine1.Trim();
            AddressLine2 = string.IsNullOrWhiteSpace(addressLine2) ? null : addressLine2.Trim();
            Locality = string.IsNullOrWhiteSpace(locality) ? null : locality.Trim();
            Region = string.IsNullOrWhiteSpace(region) ? null : region.Trim();
            Postcode = string.IsNullOrWhiteSpace(postcode) ? null : postcode.Trim();
            Landmark = string.IsNullOrWhiteSpace(landmark) ? null : landmark.Trim();
        }
    
    }
}
