using Microsoft.EntityFrameworkCore.Migrations;

namespace URLS.Infrastructure.Data.Migrations
{
    public partial class Add_field_IsMultipleAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswerId",
                table: "Questions");

            migrationBuilder.AddColumn<bool>(
                name: "IsMultipleAnswers",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMultipleAnswers",
                table: "Questions");

            migrationBuilder.AddColumn<long>(
                name: "CorrectAnswerId",
                table: "Questions",
                type: "bigint",
                nullable: true);
        }
    }
}
