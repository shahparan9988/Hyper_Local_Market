using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Common.Models;
using HyperLocalMarket.Domain.Entities;
using HyperLocalMarket.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.CreateStore
{
    public sealed class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, Guid>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStoreCommandHandler(IStoreRepository storeRepository, IUnitOfWork unitOfWork)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
        {
            var location = MapToDomain(request.Location);

            var store = new Store(request.Name, location);
            await _storeRepository.AddAsync(store, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return store.Id;
        }

        private static StoreLocation MapToDomain(StoreLocationDto dto)
        {
            var coords = new GeoLocation(dto.Coordinates.Latitude, dto.Coordinates.Longitude);

            var address = new PostalAddress(
                dto.Address.AddressLine1,
                dto.Address.AddressLine2,
                dto.Address.Locality,
                dto.Address.Region,
                dto.Address.Postcode,
                dto.Address.Landmark
            );

            var admin = new AdminArea(
                dto.AdminArea.Level1Id,
                dto.AdminArea.Level2Id,
                dto.AdminArea.Level3Id,
                dto.AdminArea.Level1,
                dto.AdminArea.Level2,
                dto.AdminArea.Level3,
                dto.AdminArea.Level4
            );

            return new StoreLocation(dto.CountryCode, coords, address, admin);
        }
    }
}
