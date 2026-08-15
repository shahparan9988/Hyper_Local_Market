using HyperLocalMarket.Domain.Authorization;
using HyperLocalMarket.Infrastructure.Persistence.Seeding.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Authorization
{
    public sealed class PlatformRoleConfiguration : IEntityTypeConfiguration<PlatformRole>
    {
        public void Configure(EntityTypeBuilder<PlatformRole> builder)
        {
            builder.ToTable("PlatformRoles");

            builder.HasKey(role => role.Id);

            builder.Property(role => role.Id)
                .ValueGeneratedNever();

            builder.Property(role => role.Code)
                .HasMaxLength(PlatformRole.CodeMaxLength)
                .IsRequired();

            builder.Property(role => role.Name)
                .HasMaxLength(PlatformRole.NameMaxLength)
                .IsRequired();

            builder.Property(role => role.Description)
                .HasMaxLength(PlatformRole.DescriptionMaxLength);

            builder.Property(role => role.IsSystem)
                .IsRequired();

            builder.Property(role => role.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasIndex(role => role.Code)
                .IsUnique()
                .HasDatabaseName("UX_PlatformRoles_Code");

            builder.HasData(
                new
                {
                    Id = AuthorizationSeedIds.PlatformAdminRole,
                    Code = PlatformRoleCodes.PlatformAdmin,
                    Name = "Platform administrator",
                    Description = "Full platform administration access.",
                    IsSystem = true,
                    CreatedAtUtc = AuthorizationSeedIds.CreatedAtUtc
                },
                new
                {
                    Id = AuthorizationSeedIds.CategoryModeratorRole,
                    Code = PlatformRoleCodes.CategoryModerator,
                    Name = "Category moderator",
                    Description = "Reviews and manages marketplace categories.",
                    IsSystem = true,
                    CreatedAtUtc = AuthorizationSeedIds.CreatedAtUtc
                });
        }
    }
}
