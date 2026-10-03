using HyperLocalMarket.Application.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Queries
{
    public interface IProductCatalogReader
    {
        Task<ProductEditorDataDto> EditorDataAsync(Guid storeId, Guid userId, CancellationToken ct);
        Task<ProductPageDto> ListAsync(Guid storeId, Guid userId, ProductListFilter filter, CancellationToken ct);
        Task<SellerProductDto> GetAsync(Guid storeId, Guid userId, Guid productId, CancellationToken ct);
    }
}
