using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLawLawContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LawContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LawLawContents",
                columns: table => new
                {
                    LawId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawLawContents", x => new { x.LawId, x.LawContentId });
                    table.ForeignKey(
                        name: "FK_LawLawContents_LawContent_LawContentId",
                        column: x => x.LawContentId,
                        principalTable: "LawContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LawLawContents_Law_LawId",
                        column: x => x.LawId,
                        principalTable: "Law",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LawLawContents_LawContentId",
                table: "LawLawContents",
                column: "LawContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LawLawContents");

            migrationBuilder.DropTable(
                name: "LawContent");
        }
    }
}
