using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedShadowStoreTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop shadow indexes if they exist
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_Store_CountryCode"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_Store_CountryCode_Store_AdminLevel2Id"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_Store_CountryCode_Store_AdminLevel3Id"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_Store_CountryCode_Store_Locality"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_Store_CountryCode_Store_Region"";");

            // Drop shadow columns if they exist
            migrationBuilder.Sql(@"ALTER TABLE ""Stores"" DROP COLUMN IF EXISTS ""Store_CountryCode"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Stores"" DROP COLUMN IF EXISTS ""Store_AdminLevel1Id"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Stores"" DROP COLUMN IF EXISTS ""Store_AdminLevel2Id"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Stores"" DROP COLUMN IF EXISTS ""Store_AdminLevel3Id"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Stores"" DROP COLUMN IF EXISTS ""Store_Locality"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Stores"" DROP COLUMN IF EXISTS ""Store_Region"";");

            // Create correct indexes on real columns
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode""
        ON ""Stores"" (""CountryCode"");
    ");

            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode_AdminLevel2Id""
        ON ""Stores"" (""CountryCode"", ""AdminLevel2Id"");
    ");

            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_CountryCode_AdminLevel3Id""
        ON ""Stores"" (""CountryCode"", ""AdminLevel3Id"");
    ");

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
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_AdminLevel2Id"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_AdminLevel3Id"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_Locality"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_CountryCode_Region"";");
        }
    }
}
