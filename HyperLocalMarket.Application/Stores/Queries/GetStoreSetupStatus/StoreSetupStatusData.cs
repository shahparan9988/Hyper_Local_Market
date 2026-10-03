using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus
{
    public sealed record StoreSetupStatusData(
        Guid Id,
        StoreStatus Status,
        string Name,
        string Slug,
        string? Description,
        string? PhoneNumber,
        string? Email,
        StoreLocationDto Location,
        string TimeZoneId,
        IReadOnlyList<StoreSetupBusinessHourData> BusinessHours,
        bool IsPickupAvailable,
        bool IsDeliveryAvailable,
        decimal? MinimumOrderAmount,
        decimal? DeliveryFee,
        decimal? DeliveryRadiusKm,
        string? LogoUrl,
        string? CoverImageUrl,
        bool IsAcceptingOrders,
        string? FulfillmentNotes = null,
        DateTime? FulfillmentConfiguredAtUtc = null)
    {
        public bool HasBusinessHours => BusinessHours.Count > 0;
    }

    public sealed record StoreSetupBusinessHourData(
        Guid Id,
        DayOfWeek DayOfWeek,
        TimeOnly? OpensAt,
        TimeOnly? ClosesAt,
        bool IsOpen24Hours);
}
