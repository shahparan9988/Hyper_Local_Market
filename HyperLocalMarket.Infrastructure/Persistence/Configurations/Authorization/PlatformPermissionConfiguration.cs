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
    public sealed class PlatformPermissionConfiguration
        : IEntityTypeConfiguration<PlatformPermission>
    {
        public void Configure(EntityTypeBuilder<PlatformPermission> builder)
        {
            builder.ToTable("PlatformPermissions");

            builder.HasKey(permission => permission.Id);

            builder.Property(permission => permission.Id)
                .ValueGeneratedNever();

            builder.Property(permission => permission.Code)
                .HasMaxLength(PlatformPermission.CodeMaxLength)
                .IsRequired();

            builder.Property(permission => permission.Description)
                .HasMaxLength(PlatformPermission.DescriptionMaxLength);

            builder.Property(permission => permission.CreatedAtUtc)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasIndex(permission => permission.Code)
                .IsUnique()
                .HasDatabaseName("UX_PlatformPermissions_Code");

            builder.HasData(
                Create(
                    AuthorizationSeedIds.RolesAssignPermission,
                    PlatformPermissionCodes.RolesAssign,
                    "Assign and revoke platform roles."),
                Create(
                    AuthorizationSeedIds.UsersViewPermission,
                    PlatformPermissionCodes.UsersView,
                    "View platform user administration data."),
                Create(
                    AuthorizationSeedIds.UsersSuspendPermission,
                    PlatformPermissionCodes.UsersSuspend,
                    "Suspend platform users."),
                Create(
                    AuthorizationSeedIds.StoresSuspendPermission,
                    PlatformPermissionCodes.StoresSuspend,
                    "Suspend marketplace stores."),
                Create(
                    AuthorizationSeedIds.CategoriesReviewPermission,
                    PlatformPermissionCodes.CategoriesReview,
                    "Review category proposals."),
                Create(
                    AuthorizationSeedIds.CategoriesManagePermission,
                    PlatformPermissionCodes.CategoriesManage,
                    "Create and modify marketplace categories."));
        }

        private static object Create(
            Guid id,
            string code,
            string description)
        {
            return new
            {
                Id = id,
                Code = code,
                Description = description,
                CreatedAtUtc = AuthorizationSeedIds.CreatedAtUtc
            };
        }
    }
}
