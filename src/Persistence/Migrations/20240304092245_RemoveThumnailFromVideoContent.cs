using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RemoveThumnailFromVideoContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumnail",
                table: "VideoContent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Thumnail",
                table: "VideoContent",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
