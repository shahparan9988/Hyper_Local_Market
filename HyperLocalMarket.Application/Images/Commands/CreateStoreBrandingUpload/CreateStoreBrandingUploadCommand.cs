using HyperLocalMarket.Domain.Images;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.CreateStoreBrandingUpload
{
    public sealed record CreateStoreBrandingUploadCommand(
        Guid StoreId,
        Guid UserId,
        StoreBrandingImageKind Kind,
        string FileName,
        string ContentType,
        long FileSizeBytes)
        : IRequest<CreateStoreBrandingUploadResult?>;

    public sealed record CreateStoreBrandingUploadResult(
        Guid UploadId,
        string Kind,
        string UploadUrl,
        string ObjectKey,
        DateTime ExpiresAtUtc);
}
