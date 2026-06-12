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
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.OwnsOne(x => x.Location, loc =>
            {
                loc.Property(x => x.CountryCode)
                    .HasColumnName("CountryCode")
                    .IsRequired()
                    .HasMaxLength(2);


                loc.OwnsOne(x => x.Coordinates, coords =>
                {
                    coords.Property(x => x.Latitude).HasColumnName("Latitude").IsRequired();
                    coords.Property(x => x.Longitude).HasColumnName("Longitude").IsRequired();
                    coords.Property(x => x.Point).HasColumnName("Location").HasColumnType("geography (point, 4326)").IsRequired(false);

                });

                loc.OwnsOne(x => x.Address, addr =>
                {
                    addr.Property(x => x.AddressLine1).HasColumnName("AddressLine1").HasMaxLength(200);
                    addr.Property(x => x.AddressLine2).HasColumnName("AddressLine2").HasMaxLength(200);
                    addr.Property(x => x.Locality).HasColumnName("Locality").HasMaxLength(200);
                    addr.Property(x => x.Region).HasColumnName("Region").HasMaxLength(200);
                    addr.Property(x => x.Postcode).HasColumnName("Postcode").HasMaxLength(20);
                    addr.Property(x => x.Landmark).HasColumnName("Landmark").HasMaxLength(300);

               
                });

                loc.OwnsOne(x => x.AdminArea, admin =>
                {
                    // IDs (canonical, index-friendly)
                    admin.Property(x => x.Level1Id).HasColumnName("AdminLevel1Id"); // e.g., DivisionId / StateId
                    admin.Property(x => x.Level2Id).HasColumnName("AdminLevel2Id"); // e.g., DistrictId / LGAId
                    admin.Property(x => x.Level3Id).HasColumnName("AdminLevel3Id"); // e.g., ThanaId / SuburbId
                    admin.Property(x => x.Level1).HasColumnName("AdminLevel1").HasMaxLength(200);
                    admin.Property(x => x.Level2).HasColumnName("AdminLevel2").HasMaxLength(200);
                    admin.Property(x => x.Level3).HasColumnName("AdminLevel3").HasMaxLength(200);
                    admin.Property(x => x.Level4).HasColumnName("AdminLevel4").HasMaxLength(200);
                });
            });

          

            //// ✅ Indexes (string-based) now work correctly
            //builder.HasIndex("CountryCode");
            //builder.HasIndex("CountryCode", "AdminLevel2Id"); // BD district-first
            //builder.HasIndex("CountryCode", "AdminLevel3Id");
            //builder.HasIndex("CountryCode", "Locality");
            //builder.HasIndex("CountryCode", "Region");
        }
    }
}
