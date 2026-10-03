using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Queries.GetStoreBrandingUploadStatus
{
    public sealed record GetStoreBrandingUploadStatusQuery(
        Guid StoreId,
        Guid UserId,
        Guid UploadId)
        : IRequest<StoreBrandingUploadStatusDto?>;

    public sealed record StoreBrandingUploadStatusDto(
        Guid UploadId,
        string Kind,
        string Status,
        string? ImageUrl,
        string? FailureReason);
}
