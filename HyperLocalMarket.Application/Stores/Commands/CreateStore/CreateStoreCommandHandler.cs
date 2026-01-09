using HyperLocalMarket.Application.Common.Interfaces.Persistence;
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
            var address = new Address(
                request.Street,
                request.Suburb,
                request.City,
                request.State,
                request.Postcode,
                request.Country
            );

            var store = new Store(request.Name, address);
            await _storeRepository.AddAsync(store, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return store.Id;
        }
    }
}
