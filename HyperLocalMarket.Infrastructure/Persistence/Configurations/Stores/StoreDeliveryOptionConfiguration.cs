using HyperLocalMarket.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Stores
{
    public sealed class StoreDeliveryOptionConfiguration : IEntityTypeConfiguration<StoreDeliveryOption>
    {
        public void Configure(EntityTypeBuilder<StoreDeliveryOption> builder)
        {
            builder.ToTable("StoreDeliveryOptions");
            builder.Ignore(x => x.DomainEvents);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasAlternateKey(x => new { x.StoreId, x.Id });
            builder.HasOne<Store>().WithMany().HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.CoverageDescription).HasMaxLength(300).IsRequired();
            builder.Property(x => x.EstimatedTimeDescription).HasMaxLength(200).IsRequired();
            builder.Property(x => x.FeeType).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
            builder.Property(x => x.CurrencyCode).HasMaxLength(3).IsRequired();
            builder.Property(x => x.Conditions).HasMaxLength(1000);
            builder.Property(x => x.Version).IsConcurrencyToken();
            builder.Property(x => x.CreatedAtUtc).HasColumnType("timestamp with time zone");
            builder.Property(x => x.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            builder.HasIndex(x => new { x.StoreId, x.IsActive });
        }
    }
}
