using HyperLocalMarket.Domain.common;
using HyperLocalMarket.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Domain.Stores
{
    public sealed class StoreDeliveryOption : AggregateRoot
    {
        public const decimal MaximumFee = 999999999.99m;
        private StoreDeliveryOption() { }

        public Guid StoreId { get; private set; }
        public string Name { get; private set; } = null!;
        public string CoverageDescription { get; private set; } = null!;
        public string EstimatedTimeDescription { get; private set; } = null!;
        public DeliveryFeeType FeeType { get; private set; }
        public decimal? FeeAmount { get; private set; }
        public string CurrencyCode { get; private set; } = null!;
        public string? Conditions { get; private set; }
        public bool IsActive { get; private set; }
        public int Version { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        public static StoreDeliveryOption Create(
            Guid storeId, string? name, string? coverageDescription,
            string? estimatedTimeDescription, DeliveryFeeType feeType,
            decimal? feeAmount, string? currencyCode, string? conditions,
            bool isActive, DateTime utcNow)
        {
            Guard.NotEmpty(storeId, nameof(storeId));
            var option = new StoreDeliveryOption { StoreId = storeId };
            option.Update(name, coverageDescription, estimatedTimeDescription,
                feeType, feeAmount, currencyCode, conditions, isActive, utcNow);
            option.CreatedAtUtc = utcNow;
            return option;
        }

        public void Update(
            string? name, string? coverageDescription, string? estimatedTimeDescription,
            DeliveryFeeType feeType, decimal? feeAmount, string? currencyCode,
            string? conditions, bool isActive, DateTime utcNow)
        {
            // Validate every input before changing any property.
            Guard.Utc(utcNow, nameof(utcNow));
            var validName = Guard.RequiredText(name, nameof(name), 100);
            var validCoverage = Guard.RequiredText(coverageDescription,
                nameof(coverageDescription), 300);
            var validTime = Guard.RequiredText(estimatedTimeDescription,
                nameof(estimatedTimeDescription), 200);
            var validConditions = Guard.OptionalText(conditions, nameof(conditions), 1000);
            var currency = Guard.RequiredText(currencyCode, nameof(currencyCode), 3)
                .ToUpperInvariant();
            if (currency is not ("BDT" or "AUD"))
                throw new DomainException("Choose BDT or AUD.");
            if (!Enum.IsDefined(feeType))
                throw new DomainException("Invalid delivery fee type.");
            if (feeType == DeliveryFeeType.ContactSeller)
            {
                if (feeAmount.HasValue)
                    throw new DomainException("Contact-seller pricing must have no amount.");
            }
            else if (!feeAmount.HasValue || feeAmount < 0 || feeAmount > MaximumFee ||
                     decimal.Round(feeAmount.Value, 2) != feeAmount.Value)
            {
                throw new DomainException(
                    "Enter a non-negative fee up to 999999999.99 with at most two decimal places.");
            }

            Name = validName;
            CoverageDescription = validCoverage;
            EstimatedTimeDescription = validTime;
            FeeType = feeType;
            FeeAmount = feeAmount;
            CurrencyCode = currency;
            Conditions = validConditions;
            IsActive = isActive;
            Version++;
            UpdatedAtUtc = utcNow;
        }

        public void SetActive(bool isActive, DateTime utcNow)
        {
            Guard.Utc(utcNow, nameof(utcNow));
            IsActive = isActive;
            Version++;
            UpdatedAtUtc = utcNow;
        }
    }
}
