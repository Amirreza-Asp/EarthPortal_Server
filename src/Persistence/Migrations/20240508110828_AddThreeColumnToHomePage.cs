using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreeColumnToHomePage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeaderAreaProtectedLandsCount",
                table: "HomePage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeaderReqCount",
                table: "HomePage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeaderUserCount",
                table: "HomePage",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderAreaProtectedLandsCount",
                table: "HomePage");

            migrationBuilder.DropColumn(
                name: "HeaderReqCount",
                table: "HomePage");

            migrationBuilder.DropColumn(
                name: "HeaderUserCount",
                table: "HomePage");
        }
    }
}
