using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Dtos
{
    public sealed record ProductImageUploadDto(
        Guid UploadId,
        string UploadUrl,
        IReadOnlyDictionary<string, string> Headers,
        DateTime ExpiresAtUtc);
    public sealed record ProductImageStatusDto(
        string Status,
        Guid? AssetId,
        string? ImageUrl,
        string? Message);
}
