using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreDeliveryOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FulfillmentConfiguredAtUtc",
                table: "Stores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfillmentNotes",
                table: "Stores",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryOptionsVersion",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Products_StoreId_Id",
                table: "Products",
                columns: new[] { "StoreId", "Id" });

            migrationBuilder.CreateTable(
                name: "StoreDeliveryOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CoverageDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    EstimatedTimeDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FeeType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FeeAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Conditions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreDeliveryOptions", x => x.Id);
                    table.UniqueConstraint("AK_StoreDeliveryOptions_StoreId_Id", x => new { x.StoreId, x.Id });
                    table.ForeignKey(
                        name: "FK_StoreDeliveryOptions_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDeliveryOptions",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDeliveryOptions", x => new { x.ProductId, x.DeliveryOptionId });
                    table.ForeignKey(
                        name: "FK_ProductDeliveryOptions_Products_StoreId_ProductId",
                        columns: x => new { x.StoreId, x.ProductId },
                        principalTable: "Products",
                        principalColumns: new[] { "StoreId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductDeliveryOptions_StoreDeliveryOptions_StoreId_Deliver~",
                        columns: x => new { x.StoreId, x.DeliveryOptionId },
                        principalTable: "StoreDeliveryOptions",
                        principalColumns: new[] { "StoreId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDeliveryOptions_StoreId_DeliveryOptionId",
                table: "ProductDeliveryOptions",
                columns: new[] { "StoreId", "DeliveryOptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDeliveryOptions_StoreId_ProductId",
                table: "ProductDeliveryOptions",
                columns: new[] { "StoreId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreDeliveryOptions_StoreId_IsActive",
                table: "StoreDeliveryOptions",
                columns: new[] { "StoreId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDeliveryOptions");

            migrationBuilder.DropTable(
                name: "StoreDeliveryOptions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Products_StoreId_Id",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FulfillmentConfiguredAtUtc",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "FulfillmentNotes",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DeliveryOptionsVersion",
                table: "Products");
        }
    }
}
