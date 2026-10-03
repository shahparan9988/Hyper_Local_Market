using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleProductOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptionValues_ProductOptions_ProductOptionId",
                table: "ProductOptionValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValues_ProductOptionV~",
                table: "ProductVariantSelections");

            migrationBuilder.DropIndex(
                name: "UX_ProductOptions_ProductId_Name",
                table: "ProductOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptionValues",
                table: "ProductOptionValues");

            migrationBuilder.RenameTable(
                name: "ProductOptionValues",
                newName: "ProductOptionValue");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptionValues_ProductOptionId",
                table: "ProductOptionValue",
                newName: "IX_ProductOptionValue_ProductOptionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsListed",
                table: "ProductOptions",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsListed",
                table: "ProductOptionValue",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptionValue",
                table: "ProductOptionValue",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UX_ProductOptions_ProductId_Name",
                table: "ProductOptions",
                columns: new[] { "ProductId", "Name" },
                unique: true,
                filter: "\"IsListed\" = TRUE");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptionValue_ProductOptions_ProductOptionId",
                table: "ProductOptionValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValue_ProductOptionVa~",
                table: "ProductVariantSelections");

            migrationBuilder.DropIndex(
                name: "UX_ProductOptions_ProductId_Name",
                table: "ProductOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptionValue",
                table: "ProductOptionValue");

            migrationBuilder.DropColumn(
                name: "IsListed",
                table: "ProductOptions");

            migrationBuilder.DropColumn(
                name: "IsListed",
                table: "ProductOptionValue");

            migrationBuilder.RenameTable(
                name: "ProductOptionValue",
                newName: "ProductOptionValues");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptionValue_ProductOptionId",
                table: "ProductOptionValues",
                newName: "IX_ProductOptionValues_ProductOptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptionValues",
                table: "ProductOptionValues",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UX_ProductOptions_ProductId_Name",
                table: "ProductOptions",
                columns: new[] { "ProductId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptionValues_ProductOptions_ProductOptionId",
                table: "ProductOptionValues",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantSelections_ProductOptionValues_ProductOptionV~",
                table: "ProductVariantSelections",
                column: "ProductOptionValueId",
                principalTable: "ProductOptionValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
