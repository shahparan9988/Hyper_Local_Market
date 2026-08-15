using HyperLocalMarket.Domain.Stores;
using HyperLocalMarket.Domain.Users;
using HyperLocalMarket.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Stores
{
    public sealed class StoreConfiguration
    : IEntityTypeConfiguration<Store>
    {
        public void Configure(
            EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");

            ConfigurePrimaryKey(builder);
            ConfigureProperties(builder);
            ConfigureLocation(builder);
            ConfigureBusinessHours(builder);
            ConfigureRelationships(builder);
            ConfigureIndexes(builder);
            ConfigureNavigationAccess(builder);
        }

        private static void ConfigurePrimaryKey(
            EntityTypeBuilder<Store> builder)
        {
            builder.HasKey(x => x.Id);

            /*
             * Store.Create() generates the Guid.
             * PostgreSQL does not generate it.
             */
            builder.Property(x => x.Id)
                .ValueGeneratedNever();
        }

        private static void ConfigureProperties(
            EntityTypeBuilder<Store> builder)
        {
            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(30);

            builder.Property(x => x.Email)
                .HasMaxLength(320);

            builder.Property(x => x.TimeZoneId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LogoUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.CoverImageUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.IsPickupAvailable)
                .IsRequired();

            builder.Property(x => x.IsDeliveryAvailable)
                .IsRequired();

            builder.Property(x => x.IsAcceptingOrders)
                .IsRequired();

            builder.Property(x => x.MinimumOrderAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.DeliveryFee)
                .HasPrecision(18, 2);

            builder.Property(x => x.DeliveryRadiusKm)
                .HasPrecision(6, 2);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnType("timestamp with time zone");
        }

        private static void ConfigureLocation(
            EntityTypeBuilder<Store> builder)
        {
            builder.OwnsOne(
                x => x.Location,
                location =>
                {
                    ConfigureCountryCode(location);
                    ConfigureCoordinates(location);
                    ConfigureAddress(location);
                    ConfigureAdminArea(location);
                });

            builder.Navigation(x => x.Location)
                .IsRequired();
        }

        private static void ConfigureCountryCode(
            OwnedNavigationBuilder<Store, StoreLocation> location)
        {
            location.Property(x => x.CountryCode)
                .HasColumnName("CountryCode")
                .HasMaxLength(2)
                .IsRequired();

            //location.HasIndex(x => x.CountryCode)
            //    .HasDatabaseName("IX_Stores_CountryCode");
        }

        private static void ConfigureCoordinates(
            OwnedNavigationBuilder<Store, StoreLocation> location)
        {
            location.OwnsOne(
                x => x.Coordinates,
                coordinates =>
                {
                    coordinates.Property(x => x.Latitude)
                        .HasColumnName("Latitude")
                        .HasColumnType("double precision")
                        .IsRequired();

                    coordinates.Property(x => x.Longitude)
                        .HasColumnName("Longitude")
                        .HasColumnType("double precision")
                        .IsRequired();

                    coordinates.Property(x => x.Point)
                        .HasColumnName("Location")
                        .HasColumnType("geography (point, 4326)")
                        .IsRequired();

                    /*
                     * PostgreSQL/PostGIS GiST index.
                     *
                     * Supports queries such as ST_DWithin and
                     * distance-based store searches.
                     */
                    coordinates.HasIndex(x => x.Point)
                        .HasDatabaseName("IX_Stores_Location_Point")
                        .HasMethod("gist");
                });

            location.Navigation(x => x.Coordinates)
                .IsRequired();
        }

        private static void ConfigureAddress(
            OwnedNavigationBuilder<Store, StoreLocation> location)
        {
            location.OwnsOne(
                x => x.Address,
                address =>
                {
                    address.Property(x => x.AddressLine1)
                        .HasColumnName("AddressLine1")
                        .HasMaxLength(300)
                        .IsRequired();

                    address.Property(x => x.AddressLine2)
                        .HasColumnName("AddressLine2")
                        .HasMaxLength(300);

                    address.Property(x => x.Locality)
                        .HasColumnName("Locality")
                        .HasMaxLength(150);

                    address.Property(x => x.Region)
                        .HasColumnName("Region")
                        .HasMaxLength(150);

                    address.Property(x => x.Postcode)
                        .HasColumnName("Postcode")
                        .HasMaxLength(30);

                    address.Property(x => x.Landmark)
                        .HasColumnName("Landmark")
                        .HasMaxLength(300);

                    address.HasIndex(x => x.Locality)
                        .HasDatabaseName("IX_Stores_Locality");

                    address.HasIndex(x => x.Region)
                        .HasDatabaseName("IX_Stores_Region");
                });

            location.Navigation(x => x.Address)
                .IsRequired();
        }

        private static void ConfigureAdminArea(
            OwnedNavigationBuilder<Store, StoreLocation> location)
        {
            location.OwnsOne(
                x => x.AdminArea,
                adminArea =>
                {
                    adminArea.Property(x => x.Level1Id)
                        .HasColumnName("AdminLevel1Id")
                        .HasMaxLength(100);

                    adminArea.Property(x => x.Level2Id)
                        .HasColumnName("AdminLevel2Id")
                        .HasMaxLength(100);

                    adminArea.Property(x => x.Level3Id)
                        .HasColumnName("AdminLevel3Id")
                        .HasMaxLength(100);

                    adminArea.Property(x => x.Level1)
                        .HasColumnName("AdminLevel1")
                        .HasMaxLength(200);

                    adminArea.Property(x => x.Level2)
                        .HasColumnName("AdminLevel2")
                        .HasMaxLength(200);

                    adminArea.Property(x => x.Level3)
                        .HasColumnName("AdminLevel3")
                        .HasMaxLength(200);

                    adminArea.Property(x => x.Level4)
                        .HasColumnName("AdminLevel4")
                        .HasMaxLength(200);

                    //adminArea.HasIndex(x => x.Level2Id)
                    //    .HasDatabaseName(
                    //        "IX_Stores_AdminLevel2Id");

                    //adminArea.HasIndex(x => x.Level3Id)
                    //    .HasDatabaseName(
                    //        "IX_Stores_AdminLevel3Id");
                });

            location.Navigation(x => x.AdminArea)
                .IsRequired();
        }

        private static void ConfigureBusinessHours(
            EntityTypeBuilder<Store> builder)
        {
            builder.OwnsMany(
                x => x.BusinessHours,
                businessHours =>
                {
                    businessHours.ToTable(
                        "StoreBusinessHours");

                    /*
                     * StoreId is a shadow property.
                     * It is the foreign key to Stores.Id.
                     */
                    businessHours.Property<Guid>("StoreId")
                        .HasColumnName("StoreId")
                        .IsRequired();

                    businessHours.WithOwner()
                        .HasForeignKey("StoreId");

                    businessHours.Property(x => x.Id)
                        .HasColumnName("Id")
                        .ValueGeneratedNever();

                    /*
                     * Composite primary key:
                     *
                     * StoreId + BusinessHourId
                     */
                    businessHours.HasKey(
                        "StoreId",
                        nameof(StoreBusinessHour.Id));

                    businessHours.Property(x => x.DayOfWeek)
                        .HasColumnName("DayOfWeek")
                        .HasConversion<string>()
                        .HasMaxLength(20)
                        .IsRequired();

                    businessHours.Property(x => x.OpensAt)
                        .HasColumnName("OpensAt")
                        .HasColumnType("time without time zone");

                    businessHours.Property(x => x.ClosesAt)
                        .HasColumnName("ClosesAt")
                        .HasColumnType("time without time zone");

                    businessHours.Property(x => x.IsOpen24Hours)
                        .HasColumnName("IsOpen24Hours")
                        .IsRequired();
                });
        }

        private static void ConfigureRelationships(
            EntityTypeBuilder<Store> builder)
        {
            /*
             * One User can own multiple Stores.
             *
             * Restrict prevents accidentally deleting all stores
             * when a user is deleted.
             */
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        private static void ConfigureIndexes(
            EntityTypeBuilder<Store> builder)
        {
            /*
             * Public store URLs require a unique slug.
             */
            //builder.HasIndex(x => x.Slug)
            //    .IsUnique()
            //    .HasDatabaseName("UX_Stores_Slug");

            /*
             * Supports:
             *
             * WHERE UserId = ...
             *
             * and:
             *
             * WHERE UserId = ... AND Status = ...
             *
             * A separate UserId index is unnecessary because UserId
             * is the first column of this composite index.
             */
            builder.HasIndex(x => new
            {
                x.UserId,
                x.Status
            })
                .HasDatabaseName(
                    "IX_Stores_UserId_Status");

            /*
             * Supports marketplace queries such as:
             *
             * WHERE Status = 'Active'
             * AND IsAcceptingOrders = true
             */
            builder.HasIndex(x => new
            {
                x.Status,
                x.IsAcceptingOrders
            })
                .HasDatabaseName(
                    "IX_Stores_Status_IsAcceptingOrders");



            builder.HasIndex(x => new
            {
                x.Status
            })
                .HasDatabaseName(
                    "IX_Stores_Status");
        }

        private static void ConfigureNavigationAccess(
            EntityTypeBuilder<Store> builder)
        {
            /*
             * Tell EF Core to use the private backing collections:
             *
             * _businessHours
             */


            builder.Navigation(x => x.BusinessHours)
                .UsePropertyAccessMode(
                    PropertyAccessMode.Field);

        }
    }

}
