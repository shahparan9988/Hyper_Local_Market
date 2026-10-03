using HyperLocalMarket.Domain.Inventory;
using HyperLocalMarket.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Inventory
{
    public sealed class InventoryAdjustmentConfiguration
        : IEntityTypeConfiguration<InventoryAdjustment>
    {
        public void Configure(EntityTypeBuilder<InventoryAdjustment> inventoryAdjustment)
        {
            inventoryAdjustment.ToTable("InventoryAdjustments");
            inventoryAdjustment.HasKey(x => x.Id);
            inventoryAdjustment.Property(x => x.Id).ValueGeneratedNever();
            inventoryAdjustment.Property(x => x.PreviousQuantity).HasPrecision(18, 3);
            inventoryAdjustment.Property(x => x.NewQuantity).HasPrecision(18, 3);
            inventoryAdjustment.Property(x => x.Reason).HasMaxLength(50);
            inventoryAdjustment.HasOne<ProductVariant>()
                .WithMany()
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            inventoryAdjustment.HasIndex(x => new { x.StoreId, x.ProductVariantId, x.CreatedAtUtc });
        }
    }

}
