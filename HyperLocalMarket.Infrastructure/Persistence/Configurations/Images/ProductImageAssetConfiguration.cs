using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Images
{
    public sealed class ProductImageAssetConfiguration
        : IEntityTypeConfiguration<ProductImageAsset>
    {
        public void Configure(EntityTypeBuilder<ProductImageAsset> asset)
        {
            asset.ToTable("ProductImageAssets");
            asset.HasKey(x => x.Id);
            asset.Property(x => x.Id).ValueGeneratedNever();
            asset.Ignore(x => x.DomainEvents);
            asset.Ignore(x => x.IsTerminal);
            asset.Property(x => x.OriginalObjectKey).HasMaxLength(1000);
            asset.Property(x => x.ProcessedObjectKey).HasMaxLength(1000);
            asset.Property(x => x.ContentType).HasMaxLength(100);
            asset.Property(x => x.PublicUrl).HasMaxLength(2000);
            asset.Property(x => x.FailureReason).HasMaxLength(500);
            asset.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            asset.Property(x => x.Version).IsConcurrencyToken();
            asset.HasIndex(x => x.OriginalObjectKey).IsUnique();
            asset.HasIndex(x => new { x.StoreId, x.CreatedAtUtc });
            asset.HasOne<Store>().WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
