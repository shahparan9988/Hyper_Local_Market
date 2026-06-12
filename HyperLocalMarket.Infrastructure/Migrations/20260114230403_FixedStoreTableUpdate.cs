using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedStoreTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Drop indexes that were created on shadow columns
            migrationBuilder.DropIndex(
                name: "IX_Stores_Store_CountryCode",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_Store_CountryCode_Store_AdminLevel2Id",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_Store_CountryCode_Store_AdminLevel3Id",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_Store_CountryCode_Store_Locality",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_Store_CountryCode_Store_Region",
                table: "Stores");

            // 2) Drop the shadow columns themselves
            migrationBuilder.DropColumn(name: "Store_CountryCode", table: "Stores");
            migrationBuilder.DropColumn(name: "Store_AdminLevel1Id", table: "Stores");
            migrationBuilder.DropColumn(name: "Store_AdminLevel2Id", table: "Stores");
            migrationBuilder.DropColumn(name: "Store_AdminLevel3Id", table: "Stores");
            migrationBuilder.DropColumn(name: "Store_Locality", table: "Stores");
            migrationBuilder.DropColumn(name: "Store_Region", table: "Stores");

            // 3) Create indexes on the REAL columns (PostgreSQL)
            // NOTE: Use IF NOT EXISTS so reruns are safe in dev environments

            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode""
        ON ""Stores"" (""CountryCode"");
    ");

            // Bangladesh: district-first (CountryCode + AdminLevel2Id)
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode_AdminLevel2Id""
        ON ""Stores"" (""CountryCode"", ""AdminLevel2Id"");
    ");

            // Optional: thana/upazila
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode_AdminLevel3Id""
        ON ""Stores"" (""CountryCode"", ""AdminLevel3Id"");
    ");

            // Address filtering
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode_Locality""
        ON ""Stores"" (""CountryCode"", ""Locality"");
    ");

            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode_Region""
        ON ""Stores"" (""CountryCode"", ""Region"");
    ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the real indexes
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_AdminLevel2Id"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_AdminLevel3Id"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_Locality"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_Region"";");

            // Recreate shadow columns (nullable, since they were nullable before)
            migrationBuilder.AddColumn<int>(
                name: "Store_AdminLevel1Id",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Store_AdminLevel2Id",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Store_AdminLevel3Id",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Store_CountryCode",
                table: "Stores",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Store_Locality",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Store_Region",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            // Recreate the old shadow indexes
            migrationBuilder.CreateIndex(
                name: "IX_Stores_Store_CountryCode",
                table: "Stores",
                column: "Store_CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Store_CountryCode_Store_AdminLevel2Id",
                table: "Stores",
                columns: new[] { "Store_CountryCode", "Store_AdminLevel2Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Store_CountryCode_Store_AdminLevel3Id",
                table: "Stores",
                columns: new[] { "Store_CountryCode", "Store_AdminLevel3Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Store_CountryCode_Store_Locality",
                table: "Stores",
                columns: new[] { "Store_CountryCode", "Store_Locality" });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Store_CountryCode_Store_Region",
                table: "Stores",
                columns: new[] { "Store_CountryCode", "Store_Region" });
        }

    }
}
