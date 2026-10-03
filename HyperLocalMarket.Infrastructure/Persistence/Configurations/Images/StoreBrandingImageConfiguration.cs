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
    public sealed class StoreBrandingImageConfiguration
        : IEntityTypeConfiguration<StoreBrandingImage>
    {
        public void Configure(
            EntityTypeBuilder<StoreBrandingImage> builder)
        {
            builder.ToTable("StoreBrandingImages");

            builder.HasKey(image => image.Id);

            builder.Property(image => image.Id)
                .ValueGeneratedNever();

            builder.Property(image => image.StoreId)
                .IsRequired();

            builder.Property(image => image.Kind)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(image => image.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(image => image.OriginalObjectKey)
                .HasMaxLength(StoreBrandingImage.ObjectKeyMaxLength)
                .IsRequired();

            builder.Property(image => image.ProcessedObjectKey)
                .HasMaxLength(StoreBrandingImage.ObjectKeyMaxLength);

            builder.Property(image => image.ContentType)
                .HasMaxLength(StoreBrandingImage.ContentTypeMaxLength)
                .IsRequired();

            builder.Property(image => image.ExpectedSizeBytes)
                .IsRequired();

            builder.Property(image => image.PublicUrl)
                .HasMaxLength(StoreBrandingImage.PublicUrlMaxLength);

            builder.Property(image => image.FailureReason)
                .HasMaxLength(StoreBrandingImage.FailureReasonMaxLength);

            builder.Property(image => image.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(image => image.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasOne<Store>()
                .WithMany()
                .HasForeignKey(image => image.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(image => new
            {
                image.StoreId,
                image.Kind
            })
                .IsUnique()
                .HasDatabaseName(
                    "UX_StoreBrandingImages_StoreId_Kind");

            builder.HasIndex(image => image.OriginalObjectKey)
                .IsUnique()
                .HasDatabaseName(
                    "UX_StoreBrandingImages_OriginalObjectKey");
        }

    }
}
