using HyperLocalMarket.Application.Images.Dtos;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Queries.GetProductImageStatus
{
    public sealed class GetProductImageStatusQueryHandler(
        IStoreRepository stores,
        IProductImageRepository images)
        : IRequestHandler<GetProductImageStatusQuery
            , ProductImageStatusDto>
    {
        public async Task<ProductImageStatusDto> Handle(GetProductImageStatusQuery request, CancellationToken ct)
        {
            //if (await stores.GetByIdAndUserIdAsync(request.StoreId, request.UserId, ct) is null)
            //    throw new NotFoundException("Store was not found.");
            var asset = await images.GetAsync(request.StoreId, request.UserId, request.UploadId, ct)
                ?? throw new NotFoundException("Upload was not found.");
            return asset.Status switch
            {
                ImageProcessingStatus.PendingUpload => new("Pending", null, null, null),
                ImageProcessingStatus.Ready => new("Ready", asset.Id, asset.PublicUrl, null),
                ImageProcessingStatus.Removed => new("Failed", null, null, "This unused upload expired. Please upload the image again."),
                _ => new(asset.Status.ToString(), null, null, asset.FailureReason)
            };
        }
    }
}
