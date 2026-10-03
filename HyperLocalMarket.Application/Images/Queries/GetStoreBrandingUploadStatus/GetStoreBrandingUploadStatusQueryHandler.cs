using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Stores.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Queries.GetStoreBrandingUploadStatus
{
    public sealed class GetStoreBrandingUploadStatusQueryHandler
    : IRequestHandler<
        GetStoreBrandingUploadStatusQuery,
        StoreBrandingUploadStatusDto?>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreBrandingImageRepository _imageRepository;

        public GetStoreBrandingUploadStatusQueryHandler(
            IStoreRepository storeRepository,
            IStoreBrandingImageRepository imageRepository)
        {
            _storeRepository = storeRepository;
            _imageRepository = imageRepository;
        }

        public async Task<StoreBrandingUploadStatusDto?> Handle(
            GetStoreBrandingUploadStatusQuery request,
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

            var image = await _imageRepository.GetByIdAndStoreIdAsync(
                request.UploadId,
                request.StoreId,
                cancellationToken);

            if (image is null)
            {
                return null;
            }

            return new StoreBrandingUploadStatusDto(
                image.Id,
                image.Kind.ToString(),
                image.Status.ToString(),
                image.PublicUrl,
                image.FailureReason);
        }
    }
}
