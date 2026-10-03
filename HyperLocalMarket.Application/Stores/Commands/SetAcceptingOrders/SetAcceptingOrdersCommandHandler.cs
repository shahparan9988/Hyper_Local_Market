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

namespace HyperLocalMarket.Application.Stores.Commands.SetAcceptingOrders
{
    public sealed class SetAcceptingOrdersCommandHandler
        : IRequestHandler<SetAcceptingOrdersCommand, StorePublicationDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public SetAcceptingOrdersCommandHandler(
            IStoreRepository storeRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<StorePublicationDto> Handle(
            SetAcceptingOrdersCommand request,
            CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAndUserIdAsTrackingAsync(
                request.StoreId, request.UserId, cancellationToken)
                ?? throw new NotFoundException("Store was not found.");

            if (store.Status != StoreStatus.Active)
            {
                throw new DomainException("Only a published store can change online ordering.");
            }

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            if (request.IsAcceptingOrders)
            {
                StorePublicationRules.EnsureReady(store);
                store.ResumeOrders(utcNow);
            }
            else
            {
                // A seller can always pause a published store, even if profile data needs fixing.
                store.PauseOrders(utcNow);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StorePublicationDto(
                store.Id,
                store.Status.ToString(),
                store.IsAcceptingOrders,
                StorePublicationRules.CanAcceptOrders(store),
                store.UpdatedAtUtc);
        }
    }

}
