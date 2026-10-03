using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Exceptions;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Stores;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.ProcessStoreBrandingImage
{
    public sealed class ProcessStoreBrandingImageCommandHandler
        : IRequestHandler<ProcessStoreBrandingImageCommand>
    {
        private readonly IStoreBrandingImageRepository _imageRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IImageStorage _imageStorage;
        private readonly IStoreBrandingImageProcessor _imageProcessor;
        private readonly IImageObjectKeyFactory _imageObjectKeyFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public ProcessStoreBrandingImageCommandHandler(
        IStoreBrandingImageRepository imageRepository,
        IStoreRepository storeRepository,
        IImageStorage imageStorage,
        IStoreBrandingImageProcessor imageProcessor,
        IImageObjectKeyFactory keyFactory,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        {
            _imageRepository = imageRepository;
            _storeRepository = storeRepository;
            _imageStorage = imageStorage;
            _imageProcessor = imageProcessor;
            _imageObjectKeyFactory = keyFactory;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task Handle(
        ProcessStoreBrandingImageCommand request,
        CancellationToken cancellationToken)
        {
            var image =
                await _imageRepository.GetByOriginalObjectKeyAsync(
                    request.OriginalObjectKey,
                    cancellationToken);

            // A previous upload was replaced or removed.
            if (image is null ||
                image.Status is ImageProcessingStatus.Ready
                    or ImageProcessingStatus.Removed)
            {
                return;
            }

            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            try
            {
                image.MarkProcessing(utcNow);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var downloaded = await _imageStorage.DownloadAsync(
                    image.OriginalObjectKey,
                    cancellationToken);

                if (downloaded.ContentLength >
                    StoreBrandingImageRules.MaximumUploadBytes)
                {
                    throw new ImageRejectedException(
                        "The uploaded image exceeds 10 MB.");
                }

                if (downloaded.ContentLength != image.ExpectedSizeBytes)
                {
                    throw new ImageRejectedException(
                        "The uploaded image size did not match the upload request.");
                }

                if (!string.Equals(
                        downloaded.ContentType,
                        image.ContentType,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new ImageRejectedException(
                        "The uploaded content type did not match the upload request.");
                }

                var processed = await _imageProcessor.ProcessAsync(
                    downloaded.Content,
                    image.Kind,
                    cancellationToken);

                //var uploadToken = Path.GetFileNameWithoutExtension(
                //    image.OriginalObjectKey);


                //var processedObjectKey =
                //    $"processed/stores/{image.StoreId:N}/branding/" +
                //    $"{kindSegment}/{image.Id}{processed.Extension}";

                var processedObjectKey = _imageObjectKeyFactory.CreateProcessedStoreBrandingKey(
                    image.StoreId,
                    image.Kind,
                    image.Id,
                    processed.Extension
                    );

                await _imageStorage.UploadProcessedAsync(
                    processedObjectKey,
                    processed.Content,
                    processed.ContentType,
                    cancellationToken);

                var store = await _storeRepository.GetByIdAsync(
                    image.StoreId,
                    cancellationToken);

                if (store is null)
                {
                    throw new ImageRejectedException(
                        "The store no longer exists.");
                }

                var publicUrl =
                    _imageStorage.CreatePublicUrl(processedObjectKey);

                if (image.Kind == StoreBrandingImageKind.Logo)
                {
                    store.UpdateImages(
                        publicUrl,
                        store.CoverImageUrl);
                }
                else
                {
                    store.UpdateImages(
                        store.LogoUrl,
                        publicUrl);
                }

                image.MarkReady(
                    processedObjectKey,
                    publicUrl,
                    _timeProvider.GetUtcNow().UtcDateTime);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (ImageRejectedException exception)
            {
                image.MarkRejected(
                    exception.Message,
                    _timeProvider.GetUtcNow().UtcDateTime);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

    }
}
