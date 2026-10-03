using HyperLocalMarket.Domain.Products;
using HyperLocalMarket.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Products
{
    public sealed class StoreProductCategoryConfiguration
        : IEntityTypeConfiguration<StoreProductCategory>
    {
        public void Configure(EntityTypeBuilder<StoreProductCategory> b)
        {
            b.ToTable(
                "StoreProductCategories",
                t => t.HasCheckConstraint(
                    "CK_StoreProductCategories_NotSelf", "\"ParentId\" IS NULL OR \"ParentId\" <> \"Id\""));
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Ignore(x => x.DomainEvents);
            b.Property(x => x.Name).HasMaxLength(100);
            b.Property(x => x.NormalizedName).HasMaxLength(100);
            b.Property(x => x.Version).IsConcurrencyToken();
            b.HasOne<Store>()
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne<StoreProductCategory>()
                .WithMany()
                .HasForeignKey(x => new { x.StoreId, x.ParentId })
                .HasPrincipalKey(x => new { x.StoreId, x.Id })
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(x => new { x.StoreId, x.NormalizedName })
                .IsUnique()
                .HasFilter("\"ParentId\" IS NULL");
            b.HasIndex(x => new { x.StoreId, x.ParentId, x.NormalizedName })
                .IsUnique()
                .HasFilter("\"ParentId\" IS NOT NULL");
        }
    }
}
