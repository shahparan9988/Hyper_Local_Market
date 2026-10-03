using HyperLocalMarket.Domain.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Application.Images.Repositories
{
    public interface IStoreBrandingImageRepository
    {
        Task<StoreBrandingImage?> GetByStoreAndKindAsync(
            Guid storeId,
            StoreBrandingImageKind kind,
            CancellationToken cancellationToken);

        Task<StoreBrandingImage?> GetByIdAndStoreIdAsync(
            Guid imageId,
            Guid storeId,
            CancellationToken cancellationToken);

        Task<StoreBrandingImage?> GetByOriginalObjectKeyAsync(
            string originalObjectKey,
            CancellationToken cancellationToken);

        Task AddAsync(
            StoreBrandingImage image,
            CancellationToken cancellationToken);
    }
}
