using HyperLocalMarket.Application.Products.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Commands.CreateStoreCategory
{
    public sealed record CreateStoreCategoryCommand(Guid StoreId, Guid UserId, Guid IdempotencyKey,
        string Name, Guid? ParentId) : IRequest<StoreCategoryDto>;
}
