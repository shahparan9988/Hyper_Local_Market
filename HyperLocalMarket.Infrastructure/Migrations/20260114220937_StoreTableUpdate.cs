using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperLocalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoreTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Address_Suburb",
                table: "Stores");

            migrationBuilder.RenameColumn(
                name: "Address_Postcode",
                table: "Stores",
                newName: "Postcode");

            migrationBuilder.AlterColumn<string>(
                name: "Postcode",
                table: "Stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminLevel1",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminLevel1Id",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminLevel2",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminLevel2Id",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminLevel3",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminLevel3Id",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminLevel4",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Stores",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Landmark",
                table: "Stores",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stores",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stores",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel1",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel1Id",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel2",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel2Id",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel3",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel3Id",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "AdminLevel4",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Landmark",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Locality",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Store_AdminLevel1Id",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Store_AdminLevel2Id",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Store_AdminLevel3Id",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Store_CountryCode",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Store_Locality",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Store_Region",
                table: "Stores");

            migrationBuilder.RenameColumn(
                name: "Postcode",
                table: "Stores",
                newName: "Address_Postcode");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Postcode",
                table: "Stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Stores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Stores",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "Stores",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Stores",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Suburb",
                table: "Stores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
