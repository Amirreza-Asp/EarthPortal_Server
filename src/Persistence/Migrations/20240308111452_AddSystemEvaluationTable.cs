using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddSystemEvaluationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemEvaluation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemEvaluation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntroductionMethod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IntroductionMethod = table.Column<int>(type: "int", nullable: false),
                    SystemEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntroductionMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntroductionMethod_SystemEvaluation_SystemEvaluationId",
                        column: x => x.SystemEvaluationId,
                        principalTable: "SystemEvaluation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemEvaluationPage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Page = table.Column<int>(type: "int", nullable: false),
                    SystemEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemEvaluationPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemEvaluationPage_SystemEvaluation_SystemEvaluationId",
                        column: x => x.SystemEvaluationId,
                        principalTable: "SystemEvaluation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntroductionMethod_SystemEvaluationId",
                table: "IntroductionMethod",
                column: "SystemEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemEvaluationPage_SystemEvaluationId",
                table: "SystemEvaluationPage",
                column: "SystemEvaluationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntroductionMethod");

            migrationBuilder.DropTable(
                name: "SystemEvaluationPage");

            migrationBuilder.DropTable(
                name: "SystemEvaluation");
        }
    }
}
