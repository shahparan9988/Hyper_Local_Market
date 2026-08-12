using HyperLocalMarket.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Products.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByStoreAndNameAsync(
            Guid storeId,
            string name,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySlugAsync(
            Guid storeId,
            string slug,
            CancellationToken cancellationToken);
    }
}
