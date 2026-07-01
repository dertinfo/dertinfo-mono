using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class competitionResultsCollatedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ResultsAreCollated",
                table: "Competitions",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultsAreCollated",
                table: "Competitions");
        }
    }
}
