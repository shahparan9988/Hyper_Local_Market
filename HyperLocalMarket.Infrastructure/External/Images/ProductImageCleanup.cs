using HyperLocalMarket.Application.Images.Services;
using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.External.Images
{
    public sealed class ProductImageCleanup(
        AppDbContext db,
        IImageStorage storage,
        IImageObjectKeyFactory objectKeys,
        TimeProvider time)
        : IProductImageCleanup
    {
        public async Task CleanupAsync(CancellationToken ct)
        {
            var cutoff = time
                .GetUtcNow()
                .UtcDateTime
                .AddDays(-7);

            var candidates =
                await db.ProductImageAssets
                .AsNoTracking()
                .Where(asset => 
                    asset.CreatedAtUtc < cutoff
                    && asset.StorageCleanedAtUtc == null
                    && !db.ProductImages.Any(i => i.AssetId == asset.Id))
                .OrderBy(x => x.CreatedAtUtc)
                .Select(x => x.Id)
                .Take(50)
                .ToListAsync(ct);

            foreach (var id in candidates)
            {
                string? originalObjectKey = null;
                string? processedObjectKey = null;
                await using (
                    var transaction =
                        await db.Database.BeginTransactionAsync(ct))
                {
                    var assets = await db.ProductImageAssets
                        .FromSqlInterpolated($"SELECT * FROM \"ProductImageAssets\" WHERE \"Id\" = {id} FOR UPDATE")
                        .ToListAsync(ct);

                    var asset = assets.SingleOrDefault();

                    if (asset is null
                        || asset.StorageCleanedAtUtc.HasValue
                        || await db.ProductImages.AnyAsync(image => image.AssetId == id, ct))
                            continue;

                    asset.Remove(
                        time.GetUtcNow().UtcDateTime);

                    originalObjectKey = 
                        asset.OriginalObjectKey;

                    // The S3 write can succeed before a database failure rolls the job back.
                    // Derive the same unique key to clean up that otherwise unrecorded output.
                    processedObjectKey = asset.ProcessedObjectKey
                        ?? objectKeys.CreateProcessedProductImageKey(
                        asset.StoreId,
                        asset.Id);

                    await db.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                }

                // Only our generated object keys are deletable by this job.
                if (processedObjectKey is not null &&
                    objectKeys.IsProcessedProductImageKey(processedObjectKey))
                {
                    await storage.DeleteAsync(originalObjectKey, ct);
                }
                    
                if (processedObjectKey is not null &&
                    objectKeys.IsProcessedProductImageKey(processedObjectKey))
                {
                    await storage.DeleteAsync(processedObjectKey, ct);
                }

                var removed = await db.ProductImageAssets
                    .SingleAsync(asset => asset.Id == id, ct);

                removed.MarkStorageCleaned(time.GetUtcNow().UtcDateTime);
                await db.SaveChangesAsync(ct);
            }
        }
    }
}
