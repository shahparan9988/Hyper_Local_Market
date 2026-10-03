using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateLocation
{
    public sealed class UpdateLocationCommandHandler
        : IRequestHandler<
            UpdateLocationCommand,
            UpdateStoreLocationResultDto?>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLocationCommandHandler(
            IStoreRepository storeRepository, IUnitOfWork unitOfWork)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateStoreLocationResultDto?> Handle(
            UpdateLocationCommand request,
            CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAndUserIdAsync(
                request.StoreId,
                request.UserId,
                cancellationToken);

            if (store is null)
            {
                return null;
            }

            // These values were checked by ValidationBehavior.
            var countryCode = request.CountryCode
                ?? throw new InvalidOperationException(
                    "Validated country code is missing.");

            var latitude = request.Latitude
                ?? throw new InvalidOperationException(
                    "Validated latitude is missing.");

            var longitude = request.Longitude
                ?? throw new InvalidOperationException(
                    "Validated longitude is missing.");

            var location = new StoreLocation(
                countryCode,
                new GeoLocation(latitude, longitude),
                new PostalAddress(
                    request.AddressLine1,
                    request.AddressLine2,
                    request.Locality,
                    request.Region,
                    request.Postcode,
                    request.Landmark),
                new AdminArea(
                    request.Level1Id,
                    request.Level2Id,
                    request.Level3Id,
                    request.Level1,
                    request.Level2,
                    request.Level3,
                    request.Level4));

            store.UpdateAddress(location);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            var savedLocation = new StoreLocationDto(
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
                    store.Location.AdminArea.Level4));

            return new UpdateStoreLocationResultDto(
                store.Id,
                savedLocation);
        }
    }
}
