using HyperLocalMarket.Domain.Images;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.RemoveStoreBrandingImage
{
    public sealed record RemoveStoreBrandingImageCommand(
        Guid StoreId,
        Guid UserId,
        StoreBrandingImageKind Kind)
        : IRequest<StoreBrandingDto?>;

    public sealed record StoreBrandingDto(
        Guid Id,
        string? LogoUrl,
        string? CoverImageUrl);
}
