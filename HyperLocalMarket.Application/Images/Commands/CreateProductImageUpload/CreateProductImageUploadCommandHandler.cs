using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Dtos;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Application.Stores.Repositories;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Shared.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.CreateProductImageUpload
{
    public sealed class CreateProductImageUploadCommandHandler(
        IStoreRepository stores,
        IProductImageRepository images,
        IImageStorage storage,
        IImageObjectKeyFactory objectKeys,
        IUnitOfWork unitOfWork,
        TimeProvider time)
        : IRequestHandler<
            CreateProductImageUploadCommand,
            ProductImageUploadDto>
    {
        public async Task<ProductImageUploadDto> Handle(
            CreateProductImageUploadCommand request,
            CancellationToken ct)
        {
            if (await stores.GetByIdAndUserIdAsync(
                request.StoreId,
                request.UserId,
                ct) is null)
            {
                throw new NotFoundException("Store was not found.");
            }

            if (string.IsNullOrWhiteSpace(request.FileName) ||
                request.FileName.Length > 255)
            {
                throw new DomainException(
                    "Provide a filename of at most 255 characters.");
            }
            var now = 
                time.GetUtcNow().UtcDateTime;

            var assetId =
                Guid.NewGuid();

            var extension =
                ProductImageRules.GetExtension(
                    request.ContentType);

            var originalObjectKey =
                objectKeys.CreateIncomingProductImageKey(
                    request.StoreId,
                    assetId,
                    extension);

            var asset =
                ProductImageAsset.Create(
                    assetId,
                    request.StoreId,
                    request.UserId,
                    originalObjectKey,
                    request.ContentType,
                    request.FileSizeBytes,
                    now);

            var expires = 
                now.AddMinutes(5);

            var url = 
                storage.CreatePresignedUploadUrl(
                    asset.OriginalObjectKey,
                    asset.ContentType,
                    expires);

            images.Add(asset);
            
            await unitOfWork.SaveChangesAsync(ct);

            return new ProductImageUploadDto(
                asset.Id,
                url,
                new Dictionary<string, string> {
                    ["Content-Type"] = asset.ContentType
                },
                expires);
        }
    }
}
