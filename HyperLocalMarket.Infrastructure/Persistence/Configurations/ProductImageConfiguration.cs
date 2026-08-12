using HyperLocalMarket.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations
{
    public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(
        EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");

            builder.HasKey(image => image.Id);

            builder.Property(image => image.Id)
                .ValueGeneratedNever();

            builder.Property(image => image.ProductId)
                .IsRequired();

            builder.Property(image => image.ProductVariantId)
                .IsRequired(false);

            builder.Property(image => image.Url)
                .HasMaxLength(ProductImage.UrlMaxLength)
                .IsRequired();

            builder.Property(image => image.AltText)
                .HasMaxLength(ProductImage.AltTextMaxLength);

            builder.Property(image => image.DisplayOrder)
                .IsRequired();

            builder.Property(image => image.IsPrimary)
                .IsRequired();

            /*
             * If a variant is deleted, the image remains attached
             * to the product but becomes a general product image.
             */
            builder.HasOne<ProductVariant>()
                .WithMany()
                .HasForeignKey(image => image.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(image => new
            {
                image.ProductId,
                image.DisplayOrder
            })
                .HasDatabaseName(
                    "IX_ProductImages_ProductId_DisplayOrder");

            /*
             * Only one general product image can be primary.
             */
            builder.HasIndex(image => image.ProductId)
                .IsUnique()
                .HasFilter(
                    "\"IsPrimary\" = TRUE AND " +
                    "\"ProductVariantId\" IS NULL")
                .HasDatabaseName(
                    "UX_ProductImages_PrimaryProductImage");

            /*
             * Only one primary image per variant.
             */
            builder.HasIndex(image => new
            {
                image.ProductId,
                image.ProductVariantId
            })
                .IsUnique()
                .HasFilter(
                    "\"IsPrimary\" = TRUE AND " +
                    "\"ProductVariantId\" IS NOT NULL")
                .HasDatabaseName(
                    "UX_ProductImages_PrimaryVariantImage");
        }
    }
}
