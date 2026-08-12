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
    public sealed class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
    {
        public void Configure(
        EntityTypeBuilder<ProductOption> builder)
        {
            builder.ToTable("ProductOptions");

            builder.HasKey(option => option.Id);

            builder.Property(option => option.Id)
                .ValueGeneratedNever();

            builder.Property(option => option.ProductId)
                .IsRequired();

            builder.Property(option => option.Name)
                .HasMaxLength(ProductOption.NameMaxLength)
                .IsRequired();

            builder.Property(option => option.DisplayOrder)
                .IsRequired();

            builder.HasMany(option => option.Values)
                .WithOne()
                .HasForeignKey(value => value.ProductOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(option => option.Values)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(option => new
            {
                option.ProductId,
                option.Name
            })
                .IsUnique()
                .HasDatabaseName(
                    "UX_ProductOptions_ProductId_Name");

            builder.HasIndex(option => new
            {
                option.ProductId,
                option.DisplayOrder
            })
                .HasDatabaseName(
                    "IX_ProductOptions_ProductId_DisplayOrder");
        }
    }
}
