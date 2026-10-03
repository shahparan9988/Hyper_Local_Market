using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Images;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.RemoveStoreBrandingImage
{
    public sealed class RemoveStoreBrandingImageCommandHandler
    : IRequestHandler<
        RemoveStoreBrandingImageCommand,
        StoreBrandingDto?>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreBrandingImageRepository _imageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RemoveStoreBrandingImageCommandHandler(
            IStoreRepository storeRepository,
            IStoreBrandingImageRepository imageRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _storeRepository = storeRepository;
            _imageRepository = imageRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<StoreBrandingDto?> Handle(
            RemoveStoreBrandingImageCommand request,
            CancellationToken cancellationToken)
        {
            var store =
                await _storeRepository.GetByIdAndUserIdAsTrackingAsync(
                    request.StoreId,
                    request.UserId,
                    cancellationToken);

            if (store is null)
            {
                return null;
            }

            if (request.Kind == StoreBrandingImageKind.Logo)
            {
                store.UpdateImages(
                    logoUrl: null,
                    coverImageUrl: store.CoverImageUrl);
            }
            else
            {
                store.UpdateImages(
                    logoUrl: store.LogoUrl,
                    coverImageUrl: null);
            }

            var image =
                await _imageRepository.GetByStoreAndKindAsync(
                    request.StoreId,
                    request.Kind,
                    cancellationToken);

            image?.MarkRemoved(
                _timeProvider.GetUtcNow().UtcDateTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StoreBrandingDto(
                store.Id,
                store.LogoUrl,
                store.CoverImageUrl);
        }
    }
}
