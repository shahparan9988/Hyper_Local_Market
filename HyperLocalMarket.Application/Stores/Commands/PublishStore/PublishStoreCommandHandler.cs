using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Application.Stores.Services;
using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.PublishStore
{
    public sealed class PublishStoreCommandHandler
        : IRequestHandler<PublishStoreCommand, StorePublicationDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public PublishStoreCommandHandler(
            IStoreRepository storeRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<StorePublicationDto> Handle(
            PublishStoreCommand request,
            CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAndUserIdAsTrackingAsync(
                request.StoreId, request.UserId, cancellationToken)
                ?? throw new NotFoundException("Store was not found.");

            // A retry after a successful publish must preserve the current order switch.
            if (store.Status != StoreStatus.Active)
            {
                if (store.Status != StoreStatus.Draft)
                {
                    throw new DomainException("Only a draft store can be published.");
                }

                StorePublicationRules.EnsureReady(store);
                store.Activate(_timeProvider.GetUtcNow().UtcDateTime);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return new StorePublicationDto(
                store.Id,
                store.Status.ToString(),
                store.IsAcceptingOrders,
                StorePublicationRules.CanAcceptOrders(store),
                store.UpdatedAtUtc);
        }
    }

}
