using HyperLocalMarket.Domain.Authorization;
using HyperLocalMarket.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Infrastructure.Persistence.Configurations.Authorization
{
    public sealed class UserPlatformRoleConfiguration
        : IEntityTypeConfiguration<UserPlatformRole>
    {
        public void Configure(EntityTypeBuilder<UserPlatformRole> builder)
        {
            builder.ToTable("UserPlatformRoles");

            builder.HasKey(assignment => assignment.Id);

            builder.Property(assignment => assignment.Id)
                .ValueGeneratedNever();

            builder.Property(assignment => assignment.UserId)
                .IsRequired();

            builder.Property(assignment => assignment.PlatformRoleId)
                .IsRequired();

            builder.Property(assignment => assignment.AssignedByUserId)
                .IsRequired();

            builder.Property(assignment => assignment.AssignedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(assignment => assignment.RevokedByUserId);

            builder.Property(assignment => assignment.RevokedAtUtc)
                .HasColumnType("timestamp with time zone");

            builder.Ignore(assignment => assignment.IsActive);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(assignment => assignment.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PlatformRole>()
                .WithMany()
                .HasForeignKey(assignment => assignment.PlatformRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(assignment => assignment.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(assignment => assignment.RevokedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Only one active copy of a role per user. Historical revoked rows remain.
            builder.HasIndex(assignment => new
            {
                assignment.UserId,
                assignment.PlatformRoleId
            })
                .IsUnique()
                .HasFilter("\"RevokedAtUtc\" IS NULL")
                .HasDatabaseName(
                    "UX_UserPlatformRoles_Active_UserId_RoleId");

            builder.HasIndex(assignment => new
            {
                assignment.PlatformRoleId,
                assignment.RevokedAtUtc
            })
                .HasDatabaseName(
                    "IX_UserPlatformRoles_RoleId_RevokedAtUtc");
        }
    }
}
