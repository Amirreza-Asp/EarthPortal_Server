using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "VideoContent",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Translator",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SystemEvaluationPage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SystemEvaluation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Role",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Publication",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "NewsCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Link",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "LawCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Law",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "IntroductionMethod",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Infographic",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Info",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Guide",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Goal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "GeoAddress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Gallery",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "FrequentlyAskedQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ExecutorManagment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "EducationalVideo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Broadcast",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Book",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Author",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Article",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ApprovalType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ApprovalStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ApprovalAuthority",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "AboutUs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "VideoContent");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Translator");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "SystemEvaluationPage");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "SystemEvaluation");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Publication");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "NewsCategory");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Link");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "LawCategory");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Law");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "IntroductionMethod");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Infographic");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Info");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Guide");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Goal");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "GeoAddress");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Gallery");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "FrequentlyAskedQuestions");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ExecutorManagment");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "EducationalVideo");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Broadcast");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Author");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ApprovalType");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ApprovalStatus");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ApprovalAuthority");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "AboutUs");
        }
    }
}
