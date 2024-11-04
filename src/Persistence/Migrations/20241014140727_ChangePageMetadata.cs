using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangePageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "PageMetadata",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PageMetadata",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PageMetadataKeywords",
                columns: table => new
                {
                    PageMetadataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageMetadataKeywords", x => new { x.PageMetadataId, x.Id });
                    table.ForeignKey(
                        name: "FK_PageMetadataKeywords_PageMetadata_PageMetadataId",
                        column: x => x.PageMetadataId,
                        principalTable: "PageMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageMetadata_Page",
                table: "PageMetadata",
                column: "Page",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageMetadataKeywords");

            migrationBuilder.DropIndex(
                name: "IX_PageMetadata_Page",
                table: "PageMetadata");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PageMetadata");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "PageMetadata",
                newName: "Value");
        }
    }
}
