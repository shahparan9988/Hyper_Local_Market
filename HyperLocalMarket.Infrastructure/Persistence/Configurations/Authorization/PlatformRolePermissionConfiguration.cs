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
    public sealed class PlatformRolePermissionConfiguration
        : IEntityTypeConfiguration<PlatformRolePermission>
    {
        public void Configure(
        EntityTypeBuilder<PlatformRolePermission> builder)
        {
            builder.ToTable("PlatformRolePermissions");

            builder.HasKey(link => new
            {
                link.PlatformRoleId,
                link.PlatformPermissionId
            });

            builder.HasOne<PlatformRole>()
                .WithMany()
                .HasForeignKey(link => link.PlatformRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<PlatformPermission>()
                .WithMany()
                .HasForeignKey(link => link.PlatformPermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(link => link.PlatformPermissionId)
                .HasDatabaseName(
                    "IX_PlatformRolePermissions_PermissionId");

            builder.HasData(
                // Platform administrator receives every permission.
                Link(AuthorizationSeedIds.PlatformAdminRole,
                    AuthorizationSeedIds.RolesAssignPermission),
                Link(AuthorizationSeedIds.PlatformAdminRole,
                    AuthorizationSeedIds.UsersViewPermission),
                Link(AuthorizationSeedIds.PlatformAdminRole,
                    AuthorizationSeedIds.UsersSuspendPermission),
                Link(AuthorizationSeedIds.PlatformAdminRole,
                    AuthorizationSeedIds.StoresSuspendPermission),
                Link(AuthorizationSeedIds.PlatformAdminRole,
                    AuthorizationSeedIds.CategoriesReviewPermission),
                Link(AuthorizationSeedIds.PlatformAdminRole,
                    AuthorizationSeedIds.CategoriesManagePermission),

                // Category moderator receives category permissions only.
                Link(AuthorizationSeedIds.CategoryModeratorRole,
                    AuthorizationSeedIds.CategoriesReviewPermission),
                Link(AuthorizationSeedIds.CategoryModeratorRole,
                    AuthorizationSeedIds.CategoriesManagePermission));
        }

        private static object Link(
            Guid roleId,
            Guid permissionId)
        {
            return new
            {
                PlatformRoleId = roleId,
                PlatformPermissionId = permissionId
            };
        }
    }
}
