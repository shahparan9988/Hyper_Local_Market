using HyperLocalMarket.Application.Common.Interfaces.Persistence;
using HyperLocalMarket.Application.Images.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.MarkProductImageFailed
{
    public sealed class MarkProductImageFailedCommandHandler(
        IProductImageRepository images,
        IUnitOfWork unitOfWork,
        TimeProvider time)
        : IRequestHandler<MarkProductImageFailedCommand>
    {
        public async Task Handle(MarkProductImageFailedCommand request, CancellationToken ct)
        {
            await using var work = await images.BeginWorkAsync(request.OriginalObjectKey, ct);

            if (work.Asset is null || work.Asset.IsTerminal)
                return;

            work.Asset.Fail(time.GetUtcNow().UtcDateTime);

            await unitOfWork.SaveChangesAsync(ct);
            await work.CommitAsync(ct);
        }
    }
}
