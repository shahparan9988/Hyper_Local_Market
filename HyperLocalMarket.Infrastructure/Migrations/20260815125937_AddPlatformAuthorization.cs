using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Products",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.CreateTable(
                name: "PlatformPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRolePermissions",
                columns: table => new
                {
                    PlatformRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformPermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRolePermissions", x => new { x.PlatformRoleId, x.PlatformPermissionId });
                    table.ForeignKey(
                        name: "FK_PlatformRolePermissions_PlatformPermissions_PlatformPermiss~",
                        column: x => x.PlatformPermissionId,
                        principalTable: "PlatformPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformRolePermissions_PlatformRoles_PlatformRoleId",
                        column: x => x.PlatformRoleId,
                        principalTable: "PlatformRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlatformRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlatformRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlatformRoles_PlatformRoles_PlatformRoleId",
                        column: x => x.PlatformRoleId,
                        principalTable: "PlatformRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPlatformRoles_users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPlatformRoles_users_RevokedByUserId",
                        column: x => x.RevokedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPlatformRoles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PlatformPermissions",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description" },
                values: new object[,]
                {
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b101"), "roles.assign", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Assign and revoke platform roles." },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b102"), "users.view", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "View platform user administration data." },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b103"), "users.suspend", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Suspend platform users." },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b104"), "stores.suspend", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Suspend marketplace stores." },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b105"), "categories.review", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Review category proposals." },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b106"), "categories.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Create and modify marketplace categories." }
                });

            migrationBuilder.InsertData(
                table: "PlatformRoles",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "IsSystem", "Name" },
                values: new object[,]
                {
                    { new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01"), "platform-admin", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Full platform administration access.", true, "Platform administrator" },
                    { new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a02"), "category-moderator", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reviews and manages marketplace categories.", true, "Category moderator" }
                });

            migrationBuilder.InsertData(
                table: "PlatformRolePermissions",
                columns: new[] { "PlatformPermissionId", "PlatformRoleId" },
                values: new object[,]
                {
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b101"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b102"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b103"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b104"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b105"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b106"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a01") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b105"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a02") },
                    { new Guid("2a74d1b6-ce4d-4456-ad23-587cde13b106"), new Guid("9d5c56e4-1b64-4b96-a8e4-e8fce1f85a02") }
                });

            migrationBuilder.CreateIndex(
                name: "UX_PlatformPermissions_Code",
                table: "PlatformPermissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRolePermissions_PermissionId",
                table: "PlatformRolePermissions",
                column: "PlatformPermissionId");

            migrationBuilder.CreateIndex(
                name: "UX_PlatformRoles_Code",
                table: "PlatformRoles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatformRoles_AssignedByUserId",
                table: "UserPlatformRoles",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatformRoles_RevokedByUserId",
                table: "UserPlatformRoles",
                column: "RevokedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlatformRoles_RoleId_RevokedAtUtc",
                table: "UserPlatformRoles",
                columns: new[] { "PlatformRoleId", "RevokedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "UX_UserPlatformRoles_Active_UserId_RoleId",
                table: "UserPlatformRoles",
                columns: new[] { "UserId", "PlatformRoleId" },
                unique: true,
                filter: "\"RevokedAtUtc\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformRolePermissions");

            migrationBuilder.DropTable(
                name: "UserPlatformRoles");

            migrationBuilder.DropTable(
                name: "PlatformPermissions");

            migrationBuilder.DropTable(
                name: "PlatformRoles");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Products",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);
        }
    }
}
