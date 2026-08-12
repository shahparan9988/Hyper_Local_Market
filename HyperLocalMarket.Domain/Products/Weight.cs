using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed record Weight
    {
        private Weight()
        {
        }

        private Weight(decimal value, WeightUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public decimal Value { get; private set; }

        public WeightUnit Unit { get; private set; }

        public static Weight Create(decimal value, WeightUnit unit)
        {
            if (value <= 0)
            {
                throw new DomainException("Weight must be greater than zero.");
            }

            if (!Enum.IsDefined(unit))
            {
                throw new DomainException("Weight unit is invalid.");
            }

            return new Weight(value, unit);
        }

    }
}
