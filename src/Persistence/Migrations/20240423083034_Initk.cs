using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EnglishPageSolution",
                table: "EnglishPageSolution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnglishPageProblem",
                table: "EnglishPageProblem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnglishPageSolution",
                table: "EnglishPageSolution",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnglishPageProblem",
                table: "EnglishPageProblem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EnglishPageSolution_EnglishPageId",
                table: "EnglishPageSolution",
                column: "EnglishPageId");

            migrationBuilder.CreateIndex(
                name: "IX_EnglishPageProblem_EnglishPageId",
                table: "EnglishPageProblem",
                column: "EnglishPageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EnglishPageSolution",
                table: "EnglishPageSolution");

            migrationBuilder.DropIndex(
                name: "IX_EnglishPageSolution_EnglishPageId",
                table: "EnglishPageSolution");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnglishPageProblem",
                table: "EnglishPageProblem");

            migrationBuilder.DropIndex(
                name: "IX_EnglishPageProblem_EnglishPageId",
                table: "EnglishPageProblem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnglishPageSolution",
                table: "EnglishPageSolution",
                columns: new[] { "EnglishPageId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnglishPageProblem",
                table: "EnglishPageProblem",
                columns: new[] { "EnglishPageId", "Id" });
        }
    }
}
