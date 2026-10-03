using HyperLocalMarket.Application.Images.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Queries.GetProductImageStatus
{
    public sealed record GetProductImageStatusQuery(
        Guid StoreId,
        Guid UserId,
        Guid UploadId) : IRequest<ProductImageStatusDto>;
}
