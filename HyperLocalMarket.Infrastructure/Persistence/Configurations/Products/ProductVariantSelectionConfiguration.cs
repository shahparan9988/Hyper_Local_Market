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
    public sealed class ProductVariantSelectionConfiguration
        : IEntityTypeConfiguration<ProductVariantSelection>
    {
        public void Configure(
        EntityTypeBuilder<ProductVariantSelection> builder)
        {
            builder.ToTable("ProductVariantSelections");

            builder.HasKey(selection => selection.Id);

            builder.Property(selection => selection.Id)
                .ValueGeneratedNever();

            builder.Property(selection => selection.ProductVariantId)
                .IsRequired();

            builder.Property(selection => selection.ProductOptionId)
                .IsRequired();

            builder.Property(selection => selection.ProductOptionValueId)
                .IsRequired();

            builder.HasOne<ProductOption>()
                .WithMany()
                .HasForeignKey(selection => selection.ProductOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ProductOptionValue>()
                .WithMany()
                .HasForeignKey(selection => selection.ProductOptionValueId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * One variant can select only one value from each option.
             */
            builder.HasIndex(selection => new
            {
                selection.ProductVariantId,
                selection.ProductOptionId
            })
                .IsUnique()
                .HasDatabaseName(
                    "UX_VariantSelections_VariantId_OptionId");
        }
    }
}
