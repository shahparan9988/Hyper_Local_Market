using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed class Dimensions
    {
        private Dimensions()
        {
        }

        private Dimensions(
            decimal length,
            decimal width,
            decimal height,
            DimensionUnit unit)
        {
            Length = length;
            Width = width;
            Height = height;
            Unit = unit;
        }

        public decimal Length { get; private set; }

        public decimal Width { get; private set; }

        public decimal Height { get; private set; }

        public DimensionUnit Unit { get; private set; }

        public static Dimensions Create(
            decimal length,
            decimal width,
            decimal height,
            DimensionUnit unit)
        {
            if (length <= 0 || width <= 0 || height <= 0)
            {
                throw new DomainException(
                    "Length, width, and height must all be greater than zero.");
            }

            if (!Enum.IsDefined(unit))
            {
                throw new DomainException("Dimension unit is invalid.");
            }

            return new Dimensions(length, width, height, unit);
        }

    }
}
