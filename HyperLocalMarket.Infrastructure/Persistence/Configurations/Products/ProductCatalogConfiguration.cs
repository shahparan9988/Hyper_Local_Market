using HyperLocalMarket.Domain.Images;
using HyperLocalMarket.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Products
{
    public sealed class ProductCatalogConfiguration : IEntityTypeConfiguration<Product>,
        IEntityTypeConfiguration<ProductVariant>, IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.Property(x => x.Version)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsConcurrencyToken();
            b.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ProductType.Product);
            b.Property(x => x.ManualAvailability)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ProductAvailability.Available);
            b.Property(x => x.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValue("BDT");
            b.Property(x => x.VariantOptionName)
                .HasMaxLength(50)
                .HasDefaultValue("");
            b.Property(x => x.TrackInventory)
                .HasDefaultValue(false);
            b.Property(x => x.IsPickupAvailable)
                .HasDefaultValue(false);
            b.HasOne<StoreProductCategory>()
                .WithMany()
                .HasForeignKey(x => new { x.StoreId, x.StoreCategoryId })
                .HasPrincipalKey(x => new { x.StoreId, x.Id })
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.StoreId, x.StoreCategoryId, x.Status });
        }
        public void Configure(EntityTypeBuilder<ProductVariant> b)
        {
            b.Property(x => x.IsPriceSet)
                .HasDefaultValue(true)
                .ValueGeneratedNever();
            b.Property(x => x.IsListed)
                .HasDefaultValue(true)
                .ValueGeneratedNever();
            b.Property(x => x.CatalogOrder).HasDefaultValue(0);
        }
        public void Configure(EntityTypeBuilder<ProductImage> b)
        {
            b.HasOne<ProductImageAsset>()
                .WithMany()
                .HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.ProductId, x.AssetId })
                .IsUnique()
                .HasFilter("\"AssetId\" IS NOT NULL");
        }
    }
}
