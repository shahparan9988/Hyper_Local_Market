using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Queries.GetStoreSetupStatus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Queries.Stores
{
    public sealed class StoreSetupStatusReader : IStoreSetupStatusReader
    {
        private readonly AppDbContext _dbContext;

        public StoreSetupStatusReader(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StoreSetupStatusData?> GetAsync(
            Guid storeId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Stores
                .AsNoTracking()
                .Where(store =>
                    store.Id == storeId &&
                    store.UserId == userId)
                .Select(store => new StoreSetupStatusData(
                    store.Id,
                    store.Status,
                    store.Name,
                    store.Slug,
                    store.Description,
                    store.PhoneNumber,
                    store.Email,

                    new StoreLocationDto(
                        store.Location.CountryCode,

                        new GeoLocationDto(
                            store.Location.Coordinates.Latitude,
                            store.Location.Coordinates.Longitude),

                        new PostalAddressDto(
                            store.Location.Address.AddressLine1,
                            store.Location.Address.AddressLine2,
                            store.Location.Address.Locality,
                            store.Location.Address.Region,
                            store.Location.Address.Postcode,
                            store.Location.Address.Landmark),

                        new AdminAreaDto(
                            store.Location.AdminArea.Level1Id,
                            store.Location.AdminArea.Level2Id,
                            store.Location.AdminArea.Level3Id,
                            store.Location.AdminArea.Level1,
                            store.Location.AdminArea.Level2,
                            store.Location.AdminArea.Level3,
                            store.Location.AdminArea.Level4)),

                    store.TimeZoneId,

                    store.BusinessHours
                        .Select(hour => new StoreSetupBusinessHourData(
                            hour.Id,
                            hour.DayOfWeek,
                            hour.OpensAt,
                            hour.ClosesAt,
                            hour.IsOpen24Hours))
                        .ToList(),

                    store.IsPickupAvailable,
                    store.IsDeliveryAvailable,
                    store.MinimumOrderAmount,
                    store.DeliveryFee,
                    store.DeliveryRadiusKm,
                    store.LogoUrl,
                    store.CoverImageUrl,
                    store.IsAcceptingOrders,
                    store.FulfillmentNotes,
                    store.FulfillmentConfiguredAtUtc))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
