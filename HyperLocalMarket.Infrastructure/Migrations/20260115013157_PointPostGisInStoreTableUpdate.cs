using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PointPostGisInStoreTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Enable PostGIS extension
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS postgis;");

            // 2) Add the new geography column
            migrationBuilder.AddColumn<object>(
                name: "Location",
                table: "Stores",
                type: "geography (point, 4326)",
                nullable: true);

            // 3) Backfill Location from existing Latitude/Longitude
            migrationBuilder.Sql(@"
        UPDATE ""Stores""
        SET ""Location"" = ST_SetSRID(ST_MakePoint(""Longitude"", ""Latitude""), 4326)::geography
        WHERE ""Longitude"" IS NOT NULL AND ""Latitude"" IS NOT NULL;
    ");

            // 4) Create spatial index (GiST)
            migrationBuilder.Sql(@"
        CREATE INDEX IF NOT EXISTS ""IX_Stores_Location""
        ON ""Stores""
        USING GIST (""Location"");
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Stores_Location"";");
            migrationBuilder.DropColumn(name: "Location", table: "Stores");
        }
    }
}
