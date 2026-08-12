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
    public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(
            EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            ConfigurePrimaryKey(builder);
            ConfigureProperties(builder);
            ConfigurePrice(builder);
            ConfigureCompareAtPrice(builder);
            ConfigureShippingWeight(builder);
            ConfigureShippingDimensions(builder);
            ConfigureSelections(builder);
            ConfigureIndexes(builder);
        }

        private static void ConfigurePrimaryKey(
            EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(variant => variant.Id);

            builder.Property(variant => variant.Id)
                .ValueGeneratedNever();
        }

        private static void ConfigureProperties(
        EntityTypeBuilder<ProductVariant> builder)
        {
            builder.Property(variant => variant.ProductId)
                .IsRequired();

            builder.Property(variant => variant.DisplayName)
                .HasMaxLength(ProductVariant.DisplayNameMaxLength)
                .IsRequired();

            builder.Property(variant => variant.Sku)
                .HasMaxLength(ProductVariant.SkuMaxLength);

            builder.Property(variant => variant.Barcode)
                .HasMaxLength(ProductVariant.BarcodeMaxLength);

            builder.Property(variant => variant.SalesUnit)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(variant => variant.MinimumOrderQuantity)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(variant => variant.QuantityIncrement)
                .HasPrecision(18, 3)
                .IsRequired();

            builder.Property(variant => variant.IsDefault)
                .IsRequired();

            builder.Property(variant => variant.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(variant => variant.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(variant => variant.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // Computed domain property, not a database column.
            builder.Ignore(variant => variant.IsActive);
        }

        private static void ConfigurePrice(
            EntityTypeBuilder<ProductVariant> builder)
        {
            builder.OwnsOne(
                variant => variant.Price,
                price =>
                {
                    price.Property(money => money.Amount)
                        .HasColumnName("PriceAmount")
                        .HasPrecision(18, 2)
                        .IsRequired();

                    price.Property(money => money.CurrencyCode)
                        .HasColumnName("PriceCurrencyCode")
                        .HasMaxLength(3)
                        .IsFixedLength()
                        .IsRequired();
                });

            builder.Navigation(variant => variant.Price)
                .IsRequired();
        }

        private static void ConfigureCompareAtPrice(
            EntityTypeBuilder<ProductVariant> builder)
        {
            builder.OwnsOne(
                variant => variant.CompareAtPrice,
                price =>
                {
                    price.Property(money => money.Amount)
                        .HasColumnName("CompareAtPriceAmount")
                        .HasPrecision(18, 2)
                        .IsRequired();

                    price.Property(money => money.CurrencyCode)
                        .HasColumnName("CompareAtPriceCurrencyCode")
                        .HasMaxLength(3)
                        .IsFixedLength()
                        .IsRequired();
                });

            builder.Navigation(variant => variant.CompareAtPrice)
                .IsRequired(false);
        }

        private static void ConfigureShippingWeight(
        EntityTypeBuilder<ProductVariant> builder)
        {
            builder.OwnsOne(
                variant => variant.ShippingWeight,
                weight =>
                {
                    weight.Property(value => value.Value)
                        .HasColumnName("ShippingWeightValue")
                        .HasPrecision(18, 3)
                        .IsRequired();

                    weight.Property(value => value.Unit)
                        .HasColumnName("ShippingWeightUnit")
                        .HasConversion<string>()
                        .HasMaxLength(20)
                        .IsRequired();
                });

            builder.Navigation(variant => variant.ShippingWeight)
                .IsRequired(false);
        }

        private static void ConfigureShippingDimensions(
            EntityTypeBuilder<ProductVariant> builder)
        {
            builder.OwnsOne(
                variant => variant.ShippingDimensions,
                dimensions =>
                {
                    dimensions.Property(value => value.Length)
                        .HasColumnName("ShippingLength")
                        .HasPrecision(18, 3)
                        .IsRequired();

                    dimensions.Property(value => value.Width)
                        .HasColumnName("ShippingWidth")
                        .HasPrecision(18, 3)
                        .IsRequired();

                    dimensions.Property(value => value.Height)
                        .HasColumnName("ShippingHeight")
                        .HasPrecision(18, 3)
                        .IsRequired();

                    dimensions.Property(value => value.Unit)
                        .HasColumnName("ShippingDimensionUnit")
                        .HasConversion<string>()
                        .HasMaxLength(20)
                        .IsRequired();
                });

            builder.Navigation(variant => variant.ShippingDimensions)
                .IsRequired(false);
        }

        private static void ConfigureSelections(
            EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasMany(variant => variant.Selections)
                .WithOne()
                .HasForeignKey(selection => selection.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(variant => variant.Selections)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }

        private static void ConfigureIndexes(
        EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasIndex(variant => new
            {
                variant.ProductId,
                variant.Sku
            })
                .IsUnique()
                .HasFilter("\"Sku\" IS NOT NULL")
                .HasDatabaseName(
                    "UX_ProductVariants_ProductId_Sku");

            builder.HasIndex(variant => new
            {
                variant.ProductId,
                variant.Barcode
            })
                .IsUnique()
                .HasFilter("\"Barcode\" IS NOT NULL")
                .HasDatabaseName(
                    "UX_ProductVariants_ProductId_Barcode");

            /*
             * A product can have only one default variant.
             */
            builder.HasIndex(variant => variant.ProductId)
                .IsUnique()
                .HasFilter("\"IsDefault\" = TRUE")
                .HasDatabaseName(
                    "UX_ProductVariants_DefaultPerProduct");

            builder.HasIndex(variant => new
            {
                variant.ProductId,
                variant.Status
            })
                .HasDatabaseName(
                    "IX_ProductVariants_ProductId_Status");
        }
    }
}
