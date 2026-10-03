using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Images;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.CreateStoreBrandingUpload
{
    public sealed class CreateStoreBrandingUploadCommandHandler
    : IRequestHandler<
        CreateStoreBrandingUploadCommand,
        CreateStoreBrandingUploadResult?>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreBrandingImageRepository _imageRepository;
        private readonly IImageStorage _imageStorage;
        private readonly IImageObjectKeyFactory _imageObjectKeyFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CreateStoreBrandingUploadCommandHandler(
            IStoreRepository storeRepository,
            IStoreBrandingImageRepository imageRepository,
            IImageStorage imageStorage,
            IImageObjectKeyFactory imageObjectKeyFactory,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _storeRepository = storeRepository;
            _imageRepository = imageRepository;
            _imageStorage = imageStorage;
            _imageObjectKeyFactory = imageObjectKeyFactory;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<CreateStoreBrandingUploadResult?> Handle(
            CreateStoreBrandingUploadCommand request,
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

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
            var imageId = Guid.NewGuid();
            var extension = StoreBrandingImageRules.GetExtension(
                request.ContentType);

            var objectKey =
                _imageObjectKeyFactory.CreateIncomingStoreBrandingKey(
                    request.StoreId,
                    request.Kind,
                    imageId,
                    extension);

            var image =
                await _imageRepository.GetByStoreAndKindAsync(
                    request.StoreId,
                    request.Kind,
                    cancellationToken);

            if (image is null)
            {
                image = StoreBrandingImage.Create(
                    imageId,
                    request.StoreId,
                    request.Kind,
                    objectKey,
                    request.ContentType,
                    request.FileSizeBytes,
                    utcNow);

                await _imageRepository.AddAsync(
                    image,
                    cancellationToken);
            }
            else
            {
                image.BeginNewUpload(
                    objectKey,
                    request.ContentType,
                    request.FileSizeBytes,
                    utcNow);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var expiresAtUtc = utcNow.AddMinutes(5);

            var uploadUrl = _imageStorage.CreatePresignedUploadUrl(
                objectKey,
                request.ContentType,
                expiresAtUtc);

            return new CreateStoreBrandingUploadResult(
                image.Id,
                image.Kind.ToString(),
                uploadUrl,
                objectKey,
                expiresAtUtc);
        }
    }
}
