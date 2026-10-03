using HyperLocalMarket.Application.Images.Repositories;
using HyperLocalMarket.Domain.Images;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Repositories.Images
{
    public sealed class StoreBrandingImageRepository
        : IStoreBrandingImageRepository
    {
        private readonly AppDbContext _dbContext;

        public StoreBrandingImageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<StoreBrandingImage?> GetByStoreAndKindAsync(
            Guid storeId,
            StoreBrandingImageKind kind,
            CancellationToken cancellationToken)
        {
            return _dbContext.StoreBrandingImages
                .SingleOrDefaultAsync(
                    image =>
                        image.StoreId == storeId &&
                        image.Kind == kind,
                    cancellationToken);
        }

        public Task<StoreBrandingImage?> GetByIdAndStoreIdAsync(
            Guid imageId,
            Guid storeId,
            CancellationToken cancellationToken)
        {
            return _dbContext.StoreBrandingImages
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    image =>
                        image.Id == imageId &&
                        image.StoreId == storeId,
                    cancellationToken);
        }

        public Task<StoreBrandingImage?> GetByOriginalObjectKeyAsync(
            string originalObjectKey,
            CancellationToken cancellationToken)
        {
            return _dbContext.StoreBrandingImages
                .SingleOrDefaultAsync(
                    image =>
                        image.OriginalObjectKey == originalObjectKey,
                    cancellationToken);
        }

        public Task AddAsync(
            StoreBrandingImage image,
            CancellationToken cancellationToken)
        {
            return _dbContext.StoreBrandingImages
                .AddAsync(image, cancellationToken)
                .AsTask();
        }
    }
}
