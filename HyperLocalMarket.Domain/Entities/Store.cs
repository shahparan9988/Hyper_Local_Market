using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Domain.Events;
using HyperLocalMarket.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Entities
{
    public class Store : Entity
    {
        public string Name { get; private set; } = default!;
        public Address Address { get; private set; } = default!;

        private Store() { }

        public Store(string name, Address address)
        {
            SetName(name);
            SetAddress(address);
            AddDomainEvent(new StoreCreatedDomainEvent(Id, Name));
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Store name is required.", nameof(name));

            Name = name.Trim();
        }

        private void SetAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Store name cannot be empty.", nameof(newName));

            Name = newName.Trim();

            //AddDomainEvent(new StoreNameChangedDomainEvent(Id, Name));
        }
    }
}
