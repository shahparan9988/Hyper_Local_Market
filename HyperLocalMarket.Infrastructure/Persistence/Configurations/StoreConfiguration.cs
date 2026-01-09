using HyperLocalMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations
{
    public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();

            b.OwnsOne(x => x.Address, a =>
            {
                a.Property(p => p.Street).HasMaxLength(200);
                a.Property(p => p.Suburb).HasMaxLength(50);
                a.Property(p => p.Postcode).HasMaxLength(10);
                a.Property(p => p.City).HasMaxLength(20);
                a.Property(p => p.State).HasMaxLength(20);
                a.Property(p => p.Country).HasMaxLength(50);
                
            });
        }
    }
}
