using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Domain.Images;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NetTopologySuite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Images
{
    public sealed class ProductImageRepository(
        AppDbContext db)
        : IProductImageRepository
    {
        public void Add(ProductImageAsset asset)
        {
            db.ProductImageAssets.Add(asset);
        }
        public Task<ProductImageAsset?> GetAsync(
            Guid storeId,
            Guid userId,
            Guid uploadId,
            CancellationToken cancellationToken)
        {
            return db.ProductImageAssets
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    asset =>
                        asset.StoreId == storeId &&
                        asset.UserId == userId &&
                        asset.Id == uploadId,
                    cancellationToken);
        }
            
        public async Task<IProductImageWork> BeginWorkAsync(
            string originalObjectKey,
            CancellationToken cancellationToken)
        {
            var transaction =
                await db.Database.BeginTransactionAsync(
                    cancellationToken);
            try
            {
                // PostgreSQL row-level lock.
                // Keep this query simple and do not compose additional LINQ
                // over the raw SQL locking query.

                var assets =
                    await db.ProductImageAssets
                        .FromSqlInterpolated(
                            $"SELECT * FROM \"ProductImageAssets\" WHERE \"OriginalObjectKey\" = {originalObjectKey} FOR UPDATE")
                        .AsTracking()
                        .ToListAsync(cancellationToken);

                var asset = assets.SingleOrDefault();

                return new ProductImageWork(
                    transaction,
                    asset);
            }
            catch {
                await transaction.DisposeAsync();
                throw;
            }
        }
        private sealed class ProductImageWork(
            IDbContextTransaction transaction,
            ProductImageAsset? asset)
            : IProductImageWork
        {
            public ProductImageAsset? Asset => asset;
            public Task CommitAsync(
                CancellationToken cancellationToken)
            {
                return transaction.CommitAsync(cancellationToken);
            } 
            public ValueTask DisposeAsync()
            {
                return transaction.DisposeAsync();
            }
        }
    }

}
