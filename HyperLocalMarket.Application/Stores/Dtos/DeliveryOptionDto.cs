using HyperLocalMarket.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Dtos
{
    public sealed record DeliveryOptionDto(
        Guid Id, Guid StoreId, string Name, string CoverageDescription,
        string EstimatedTimeDescription, string FeeType, decimal? FeeAmount,
        string CurrencyCode, string? Conditions, bool IsActive, int Version)
    {
        public static DeliveryOptionDto From(StoreDeliveryOption value) => new(
            value.Id, value.StoreId, value.Name, value.CoverageDescription,
            value.EstimatedTimeDescription, value.FeeType.ToString(), value.FeeAmount,
            value.CurrencyCode, value.Conditions, value.IsActive, value.Version);
    }

    public sealed record DeliveryOptionInput(
        string? Name, string? CoverageDescription, string? EstimatedTimeDescription,
        string? FeeType, decimal? FeeAmount, string? CurrencyCode,
        string? Conditions, bool IsActive);

    public sealed record FulfillmentDto(
        Guid Id, bool IsPickupAvailable, bool IsDeliveryAvailable,
        string? FulfillmentNotes, DateTime? FulfillmentConfiguredAtUtc);
}
