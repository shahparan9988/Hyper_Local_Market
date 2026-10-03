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
    public sealed class InventoryItemConfiguration
        : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> inventoryItem)
        {
            inventoryItem.ToTable("InventoryItems");
            inventoryItem.HasKey(x => x.Id);
            inventoryItem.Property(x => x.Id).ValueGeneratedNever();
            inventoryItem.Ignore(x => x.DomainEvents);
            inventoryItem.Ignore(x => x.AvailableQuantity);
            inventoryItem.Ignore(x => x.IsLowStock);
            inventoryItem.Ignore(x => x.IsOutOfStock);
            inventoryItem.Property(x => x.OnHandQuantity).HasPrecision(18, 3);
            inventoryItem.Property(x => x.ReservedQuantity).HasPrecision(18, 3);
            inventoryItem.Property(x => x.ReorderPoint).HasPrecision(18, 3);
            inventoryItem.Property(x => x.Version).IsConcurrencyToken();
            inventoryItem.HasOne<ProductVariant>()
                .WithOne()
                .HasForeignKey<InventoryItem>(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
