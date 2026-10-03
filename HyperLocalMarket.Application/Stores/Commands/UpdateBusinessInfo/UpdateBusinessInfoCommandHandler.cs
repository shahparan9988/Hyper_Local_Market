using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Stores.Dtos;
using HyperLocalMarket.Application.Stores.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Stores.Commands.UpdateBusinessInfo
{
    public sealed class UpdateBusinessInfoCommandHandler
        : IRequestHandler<UpdateBusinessInfoCommand, StoreBusinessInfoDto?>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBusinessInfoCommandHandler(
            IStoreRepository storeRepository, IUnitOfWork unitOfWork)
        {
            _storeRepository = storeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreBusinessInfoDto?> Handle(
            UpdateBusinessInfoCommand request,
            CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByIdAndUserIdAsTrackingAsync(
                request.StoreId,
                request.UserId,
                cancellationToken);

            if (store is null)
            {
                return null;
            }

            store.UpdateBusinessInfo(
                request.Name,
                request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StoreBusinessInfoDto(
                store.Id,
                store.Name,
                store.Slug,
                store.Description);
        }
    }
}
