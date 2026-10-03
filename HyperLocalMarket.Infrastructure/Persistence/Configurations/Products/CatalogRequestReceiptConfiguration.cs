using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Infrastructure.Persistence.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Products
{
    public sealed class CatalogRequestReceiptConfiguration 
        : IEntityTypeConfiguration<CatalogRequestReceipt>
    {
        public void Configure(EntityTypeBuilder<CatalogRequestReceipt> b)
        {
            b.ToTable("CatalogRequestReceipts");
            b.HasKey(x => new {
                x.StoreId,
                x.UserId,
                x.Operation,
                x.RequestKey 
            });
            b.Property(x => x.Operation).HasMaxLength(50);
            b.Property(x => x.Hash).HasMaxLength(64);
            b.Property(x => x.ResultJson).HasColumnType("text");
            b.HasOne<Store>()
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
