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
    public sealed class ProductDeliveryOptionConfiguration : IEntityTypeConfiguration<ProductDeliveryOption>
    {
        public void Configure(EntityTypeBuilder<ProductDeliveryOption> builder)
        {
            builder.ToTable("ProductDeliveryOptions");
            builder.HasKey(x => new { x.ProductId, x.DeliveryOptionId });

            // Matching StoreId in BOTH foreign keys prevents cross-store links at DB level.
            builder.HasOne<Product>().WithMany(product => product.DeliveryOptions)
                .HasForeignKey(x => new { x.StoreId, x.ProductId })
                .HasPrincipalKey(x => new { x.StoreId, x.Id })
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<StoreDeliveryOption>().WithMany()
                .HasForeignKey(x => new { x.StoreId, x.DeliveryOptionId })
                .HasPrincipalKey(x => new { x.StoreId, x.Id })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
