using HyperLocalMarket.Domain.Categories;
using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            ConfigurePrimaryKey(builder);
            ConfigureProperties(builder);
            ConfigureRelationships(builder);
            ConfigureIndexes(builder);
            ConfigureNavigationAccess(builder);
        }

        private static void ConfigurePrimaryKey(
        EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(product => product.Id);

            // Entity generates the Guid.
            builder.Property(product => product.Id)
                .ValueGeneratedNever();
        }

        private static void ConfigureProperties(
        EntityTypeBuilder<Product> builder)
        {
            builder.Property(product => product.StoreId)
                .IsRequired();

            builder.Property(product => product.CategoryId)
                .IsRequired(false);

            builder.Property(product => product.CategoryProposalId)
                .IsRequired(false);

            builder.Property(product => product.CategoryAssignmentStatus)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(product => product.Name)
                .HasMaxLength(Product.NameMaxLength)
                .IsRequired();

            builder.Property(product => product.Slug)
                .HasMaxLength(Product.SlugMaxLength)
                .IsRequired();

            builder.Property(product => product.Description)
                .HasMaxLength(Product.DescriptionMaxLength);

            builder.Property(product => product.Brand)
                .HasMaxLength(Product.BrandMaxLength);

            builder.Property(product => product.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(product => product.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(product => product.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(product => product.PublishedAtUtc)
                .HasColumnType("timestamp with time zone");

            builder.Property(product => product.ArchivedAtUtc)
                .HasColumnType("timestamp with time zone");
        }

        private static void ConfigureRelationships(
        EntityTypeBuilder<Product> builder)
        {
            builder.HasOne<Store>()
                .WithMany()
                .HasForeignKey(product => product.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Product.CategoryProposalId represents the proposal
             * currently linked to the product.
             */
            builder.HasOne<CategoryProposal>()
                .WithOne()
                .HasForeignKey<Product>(
                    product => product.CategoryProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(product => product.Options)
                .WithOne()
                .HasForeignKey(option => option.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(product => product.Variants)
                .WithOne()
                .HasForeignKey(variant => variant.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(product => product.Images)
                .WithOne()
                .HasForeignKey(image => image.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureIndexes(
        EntityTypeBuilder<Product> builder)
        {
            builder.HasIndex(product => new
            {
                product.StoreId,
                product.Name
            })
                .IsUnique()
                .HasDatabaseName(
                    "UX_Products_StoreId_Name");

            builder.HasIndex(product => new
            {
                product.StoreId,
                product.Slug
            })
                .IsUnique()
                .HasDatabaseName(
                    "UX_Products_StoreId_Slug");

            /*
             * Supports:
             *
             * WHERE StoreId = ...
             * AND Status = 'Active'
             */
            builder.HasIndex(product => new
            {
                product.StoreId,
                product.Status
            })
                .HasDatabaseName(
                    "IX_Products_StoreId_Status");

            builder.HasIndex(product => new
            {
                product.CategoryId,
                product.Status
            })
                .HasDatabaseName(
                    "IX_Products_CategoryId_Status");

            builder.HasIndex(product => product.CategoryProposalId)
                .IsUnique()
                .HasFilter(
                    "\"CategoryProposalId\" IS NOT NULL")
                .HasDatabaseName(
                    "UX_Products_CategoryProposalId");
        }

        private static void ConfigureNavigationAccess(
        EntityTypeBuilder<Product> builder)
        {
            builder.Navigation(product => product.Options)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(product => product.Variants)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(product => product.Images)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
