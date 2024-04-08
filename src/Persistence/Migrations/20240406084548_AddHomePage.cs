using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHomePage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomePage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeaderTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderPortBtnEnable = table.Column<bool>(type: "bit", nullable: false),
                    HeaderAppBtnEnable = table.Column<bool>(type: "bit", nullable: false),
                    WorkTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkPort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkApp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePage", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomePage");
        }
    }
}
