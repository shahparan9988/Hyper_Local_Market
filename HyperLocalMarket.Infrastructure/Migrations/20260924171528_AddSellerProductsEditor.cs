using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSellerProductsEditor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptionValue_ProductOptions_ProductOptionId",
                table: "ProductOptionValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValue_ProductOptionVa~",
                table: "ProductVariantSelections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptionValue",
                table: "ProductOptionValue");

            migrationBuilder.RenameTable(
                name: "ProductOptionValue",
                newName: "ProductOptionValues");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptionValue_ProductOptionId",
                table: "ProductOptionValues",
                newName: "IX_ProductOptionValues_ProductOptionId");

            migrationBuilder.AddColumn<int>(
                name: "CatalogOrder",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsListed",
                table: "ProductVariants",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceSet",
                table: "ProductVariants",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Products",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "BDT");

            migrationBuilder.AddColumn<bool>(
                name: "IsPickupAvailable",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ManualAvailability",
                table: "Products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Available");

            migrationBuilder.AddColumn<Guid>(
                name: "StoreCategoryId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrackInventory",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Product");

            migrationBuilder.AddColumn<string>(
                name: "VariantOptionName",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "ProductImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptionValues",
                table: "ProductOptionValues",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CatalogRequestReceipts",
                columns: table => new
                {
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Operation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequestKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ResultJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogRequestReceipts", x => new { x.StoreId, x.UserId, x.Operation, x.RequestKey });
                    table.ForeignKey(
                        name: "FK_CatalogRequestReceipts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    NewQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackInventory = table.Column<bool>(type: "boolean", nullable: false),
                    AllowBackorder = table.Column<bool>(type: "boolean", nullable: false),
                    OnHandQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    ReservedQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    ReorderPoint = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImageAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalObjectKey = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ProcessedObjectKey = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExpectedSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PublicUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StorageCleanedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImageAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImageAssets_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductCategories", x => x.Id);
                    table.UniqueConstraint("AK_StoreProductCategories_StoreId_Id", x => new { x.StoreId, x.Id });
                    table.CheckConstraint("CK_StoreProductCategories_NotSelf", "\"ParentId\" IS NULL OR \"ParentId\" <> \"Id\"");
                    table.ForeignKey(
                        name: "FK_StoreProductCategories_StoreProductCategories_StoreId_Paren~",
                        columns: x => new { x.StoreId, x.ParentId },
                        principalTable: "StoreProductCategories",
                        principalColumns: new[] { "StoreId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreProductCategories_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId_StoreCategoryId_Status",
                table: "Products",
                columns: new[] { "StoreId", "StoreCategoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_AssetId",
                table: "ProductImages",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_AssetId",
                table: "ProductImages",
                columns: new[] { "ProductId", "AssetId" },
                unique: true,
                filter: "\"AssetId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_ProductVariantId",
                table: "InventoryAdjustments",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_StoreId_ProductVariantId_CreatedAtUtc",
                table: "InventoryAdjustments",
                columns: new[] { "StoreId", "ProductVariantId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ProductVariantId",
                table: "InventoryItems",
                column: "ProductVariantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImageAssets_OriginalObjectKey",
                table: "ProductImageAssets",
                column: "OriginalObjectKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImageAssets_StoreId_CreatedAtUtc",
                table: "ProductImageAssets",
                columns: new[] { "StoreId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductCategories_StoreId_NormalizedName",
                table: "StoreProductCategories",
                columns: new[] { "StoreId", "NormalizedName" },
                unique: true,
                filter: "\"ParentId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductCategories_StoreId_ParentId_NormalizedName",
                table: "StoreProductCategories",
                columns: new[] { "StoreId", "ParentId", "NormalizedName" },
                unique: true,
                filter: "\"ParentId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductImageAssets_AssetId",
                table: "ProductImages",
                column: "AssetId",
                principalTable: "ProductImageAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptionValues_ProductOptions_ProductOptionId",
                table: "ProductOptionValues",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_StoreProductCategories_StoreId_StoreCategoryId",
                table: "Products",
                columns: new[] { "StoreId", "StoreCategoryId" },
                principalTable: "StoreProductCategories",
                principalColumns: new[] { "StoreId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValues_ProductOptionV~",
                table: "ProductVariantSelections",
                column: "ProductOptionValueId",
                principalTable: "ProductOptionValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            HyperLocalMarket.Infrastructure.Migrations.SellerProductsDataBackfill.Apply(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductImageAssets_AssetId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptionValues_ProductOptions_ProductOptionId",
                table: "ProductOptionValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_StoreProductCategories_StoreId_StoreCategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValues_ProductOptionV~",
                table: "ProductVariantSelections");

            migrationBuilder.DropTable(
                name: "CatalogRequestReceipts");

            migrationBuilder.DropTable(
                name: "InventoryAdjustments");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "ProductImageAssets");

            migrationBuilder.DropTable(
                name: "StoreProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId_StoreCategoryId_Status",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_AssetId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId_AssetId",
                table: "ProductImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptionValues",
                table: "ProductOptionValues");

            migrationBuilder.DropColumn(
                name: "CatalogOrder",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "IsListed",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "IsPriceSet",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsPickupAvailable",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ManualAvailability",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoreCategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TrackInventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VariantOptionName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "ProductImages");

            migrationBuilder.RenameTable(
                name: "ProductOptionValues",
                newName: "ProductOptionValue");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptionValues_ProductOptionId",
                table: "ProductOptionValue",
                newName: "IX_ProductOptionValue_ProductOptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptionValue",
                table: "ProductOptionValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptionValue_ProductOptions_ProductOptionId",
                table: "ProductOptionValue",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValue_ProductOptionVa~",
                table: "ProductVariantSelections",
                column: "ProductOptionValueId",
                principalTable: "ProductOptionValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
