using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGeoAddressProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "GeoAddress");

            migrationBuilder.DropColumn(
                name: "Lon",
                table: "GeoAddress");

            migrationBuilder.AddColumn<string>(
                name: "IFrame",
                table: "GeoAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IFrame",
                table: "GeoAddress");

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "GeoAddress",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Lon",
                table: "GeoAddress",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
