using HyperLocalMarket.Application.Images.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Commands.CreateProductImageUpload
{
    public sealed record CreateProductImageUploadCommand(
        Guid StoreId,
        Guid UserId,
        string FileName,
        string ContentType,
        long FileSizeBytes) : IRequest<ProductImageUploadDto>;
}
