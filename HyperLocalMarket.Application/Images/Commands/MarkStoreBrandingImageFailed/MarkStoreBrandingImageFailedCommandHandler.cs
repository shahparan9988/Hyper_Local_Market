using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Domain.Images;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.MarkStoreBrandingImageFailed
{
    public sealed class MarkStoreBrandingImageFailedCommandHandler
    : IRequestHandler<MarkStoreBrandingImageFailedCommand>
    {
        private readonly IStoreBrandingImageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public MarkStoreBrandingImageFailedCommandHandler(
            IStoreBrandingImageRepository repository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task Handle(
            MarkStoreBrandingImageFailedCommand request,
            CancellationToken cancellationToken)
        {
            var image = await _repository.GetByOriginalObjectKeyAsync(
                request.OriginalObjectKey,
                cancellationToken);

            if (image is null ||
                image.Status is ImageProcessingStatus.Ready
                    or ImageProcessingStatus.Rejected
                    or ImageProcessingStatus.Removed)
            {
                return;
            }

            image.MarkFailed(
                request.Reason[..Math.Min(request.Reason.Length, 500)],
                _timeProvider.GetUtcNow().UtcDateTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
