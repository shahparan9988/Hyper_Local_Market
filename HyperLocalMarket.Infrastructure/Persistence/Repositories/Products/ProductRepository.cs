using HyperLocalMarket.Application.Products.Repositories;
using HyperLocalMarket.Domain.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Products
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Include(product => product.Options)
                    .ThenInclude(option => option.Values)
                .Include(product => product.Variants)
                    .ThenInclude(variant => variant.Selections)
                .Include(product => product.Images)
                .AsSplitQuery()
                .SingleOrDefaultAsync(
                    product => product.Id == productId,
                    cancellationToken);
        }

        public async Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(
                product,
                cancellationToken);
        }

        public async Task<bool> ExistsByStoreAndNameAsync(
            Guid storeId,
            string name,
            CancellationToken cancellationToken = default)
        {
            var normalizedName = name.Trim().ToLower();

            return await _dbContext.Products.AnyAsync(
                product =>
                    product.StoreId == storeId &&
                    product.Name.ToLower() == normalizedName,
                cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(
            Guid storeId,
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .AnyAsync(
                    product =>
                        product.StoreId == storeId &&
                        product.Slug == slug,
                    cancellationToken);
        }
    }
}
