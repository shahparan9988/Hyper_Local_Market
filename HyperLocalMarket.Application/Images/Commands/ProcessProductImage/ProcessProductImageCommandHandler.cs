using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Exceptions;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Application.Images.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.ProcessProductImage
{
    public sealed class ProcessProductImageCommandHandler(
        IProductImageRepository images,
        IImageStorage storage,
        IProductPhotoEncoder encoder,
        IImageObjectKeyFactory keyFactory,
        IUnitOfWork unitOfWork,
        TimeProvider time)
        : IRequestHandler<ProcessProductImageCommand>
    {
        public async Task Handle(ProcessProductImageCommand request, CancellationToken ct)
        {
            // A per-asset database row lock serializes duplicate SQS deliveries, including
            // overlapping workers. Only the first successful job writes the immutable result.

            await using var work = 
                await images.BeginWorkAsync(
                    request.OriginalObjectKey,
                    ct);

            var asset = work.Asset;

            if (asset is null || asset.IsTerminal)
                return;
            try
            {
                asset.MarkProcessing(
                    time.GetUtcNow().UtcDateTime);

                var original = 
                    await storage.DownloadAsync(
                        asset.OriginalObjectKey,
                        ct);

                if (original.Content.LongLength != asset.ExpectedSizeBytes ||
                    original.ContentLength != asset.ExpectedSizeBytes)
                {
                    throw new ImageRejectedException(
                        "The uploaded size does not match the upload request.");
                }
                    
                if (!string.Equals(
                    original.ContentType,
                    asset.ContentType,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new ImageRejectedException(
                        "The uploaded content type does not match the request.");
                }
                    
                var processed = 
                    await encoder.EncodeAsync(
                        original.Content,
                        asset.ContentType,
                        ct);

                var key = keyFactory.CreateProcessedProductImageKey(
                    asset.StoreId,
                    asset.Id);

                await storage.UploadProcessedAsync(
                    key,
                    processed.Content,
                    processed.ContentType,
                    ct);

                asset.MarkReady(
                    key,
                    storage.CreatePublicUrl(key),
                    time.GetUtcNow().UtcDateTime);
            }
            catch (ImageRejectedException error) {
                asset.Reject(
                    error.Message,
                    time.GetUtcNow().UtcDateTime);
            }

            await unitOfWork.SaveChangesAsync(ct);

            await work.CommitAsync(ct);
        }
    }

}
