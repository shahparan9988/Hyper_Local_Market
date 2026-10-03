using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Application.Stores.Services;
using HyperLocalMarket.Domain.Stores;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus
{
    public sealed class GetStoreSetupStatusQueryHandler
        : IRequestHandler<GetStoreSetupStatusQuery, StoreSetupStatusDto?>
    {
        private readonly IStoreSetupStatusReader _setupStatusReader;

        public GetStoreSetupStatusQueryHandler(IStoreSetupStatusReader setupStatusReader)
        {
            _setupStatusReader = setupStatusReader;
        }

        public async Task<StoreSetupStatusDto?> Handle(
            GetStoreSetupStatusQuery request,
            CancellationToken cancellationToken)
        {
            var store = await _setupStatusReader.GetAsync(
                request.StoreId, request.UserId, cancellationToken);

            if (store is null)
            {
                return null;
            }

            // The checklist and the publish command use the same saved-data rules.
            var errors = StorePublicationRules.GetErrors(store);

            var sections = new List<StoreSetupSectionDto>
        {
            RequiredSection("business-info", "Business information", errors, "name"),
            RequiredSection("location", "Location", errors,
                "countryCode", "addressLine1", "latitude", "longitude"),
            RequiredSection("contact", "Contact", errors, "phoneNumber", "timeZoneId"),
            OptionalSection("branding", "Branding",
                !string.IsNullOrWhiteSpace(store.LogoUrl), "logoUrl"),
            OptionalSection("business-hours", "Business hours",
                store.HasBusinessHours, "businessHours"),
            OptionalSection("pickup-delivery", "Pickup and delivery",
                store.FulfillmentConfiguredAtUtc.HasValue, "fulfillmentSettings")
        };

            sections = sections.Select(section => section with
            {
                Data = CreateSectionData(section.Key, store)
            }).ToList();

            var requiredSections = sections.Where(section => section.IsRequired).ToList();
            var requiredCompleted = requiredSections.Count(section => section.IsComplete);
            var requiredSetupComplete = requiredCompleted == requiredSections.Count;

            var canPublish = store.Status == StoreStatus.Draft && requiredSetupComplete;

            // This is eligibility to turn orders ON. It is not the current switch
            // and is not an "open now" calculation based on the business schedule.
            var canAcceptOrders = store.Status == StoreStatus.Active && requiredSetupComplete;

            var nextRecommendedSection = sections
                .FirstOrDefault(section => section.IsRequired && !section.IsComplete)?.Key
                ?? sections.FirstOrDefault(section => !section.IsComplete)?.Key;

            return new StoreSetupStatusDto(
                store.Id,
                store.Status.ToString(),
                sections,
                sections.Count(section => section.IsComplete),
                sections.Count,
                requiredCompleted,
                requiredSections.Count,
                canPublish,
                canAcceptOrders,
                nextRecommendedSection);
        }

        private static StoreSetupSectionDto RequiredSection(
            string key,
            string label,
            IReadOnlyDictionary<string, string[]> errors,
            params string[] fields)
        {
            var missingFields = fields.Where(errors.ContainsKey).ToList();
            return new StoreSetupSectionDto(key, label, true,
                missingFields.Count == 0, missingFields);
        }

        private static StoreSetupSectionDto OptionalSection(
            string key, string label, bool complete, string missingField)
        {
            IReadOnlyList<string> missingFields = complete ? [] : [missingField];
            return new StoreSetupSectionDto(key, label, false, complete, missingFields);
        }

        private static object CreateSectionData(string key, StoreSetupStatusData store)
        {
            return key switch
            {
                "business-info" => new
                {
                    store.Name,
                    store.Slug,
                    store.Description
                },
                "contact" => new
                {
                    store.PhoneNumber,
                    store.Email,
                    store.TimeZoneId
                },
                "location" => new
                {
                    store.Location,
                    store.TimeZoneId
                },
                "business-hours" => new
                {
                    BusinessHours = store.BusinessHours
                        .OrderBy(hour => hour.DayOfWeek)
                        .ThenBy(hour => hour.OpensAt)
                        .Select(hour => new
                        {
                            hour.Id,
                            DayOfWeek = (int)hour.DayOfWeek,
                            hour.OpensAt,
                            hour.ClosesAt,
                            hour.IsOpen24Hours
                        }).ToList()
                },
                "pickup-delivery" => new
                {
                    store.IsPickupAvailable,
                    store.IsDeliveryAvailable,
                    store.FulfillmentNotes,
                    store.FulfillmentConfiguredAtUtc,
                    store.IsAcceptingOrders
                },
                "branding" => new
                {
                    store.LogoUrl,
                    store.CoverImageUrl
                },
                _ => throw new ArgumentOutOfRangeException(nameof(key), key,
                    "Unknown store setup section.")
            };
        }
    }
}
