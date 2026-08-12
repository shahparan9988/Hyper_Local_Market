using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Products
{
    public sealed record Money
    {
        private Money()
        {
        }

        private Money(decimal amount, string currencyCode)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
        }

        public decimal Amount { get; private set; }

        public string CurrencyCode { get; private set; } = null!;

        public static Money Create(decimal amount, string currencyCode)
        {
            if (amount < 0)
            {
                throw new DomainException("Money amount cannot be negative.");
            }

            var normalizedCurrency = Guard.RequiredText(
                    currencyCode,
                    nameof(currencyCode),
                    3)
                .ToUpperInvariant();

            if (normalizedCurrency.Length != 3 ||
                normalizedCurrency.Any(character => character is < 'A' or > 'Z'))
            {
                throw new DomainException(
                    "Currency code must be a three-letter ISO 4217 code.");
            }

            return new Money(amount, normalizedCurrency);
        }

        public bool HasSameCurrency(Money other)
        {
            ArgumentNullException.ThrowIfNull(other);

            return string.Equals(
                CurrencyCode,
                other.CurrencyCode,
                StringComparison.Ordinal);
        }

    }
}
