using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.ValueObjects
{
    public sealed class Address
    {
        public string Street { get; }
        public string Suburb { get; }
        public string Postcode { get; }
        public string City {  get; }
        public string State { get; }
        public string Country { get; }

        private Address() { }
        public Address(string street, string suburb, string postcode, string city, string state, string country)
        {
            Street = street ?? throw new ArgumentNullException(nameof(street));
            Suburb = suburb ?? throw new ArgumentNullException( nameof(suburb));
            Postcode = postcode ?? throw new ArgumentNullException(nameof(postcode));
            City = city ?? throw new ArgumentNullException(nameof(city));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }

     
    }
}
