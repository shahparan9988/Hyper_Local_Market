using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDevicKeyToSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceKey",
                table: "sessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceKey",
                table: "sessions");
        }
    }
}
