using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertofDertsUpdate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChampionship",
                table: "DodSubmission",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "DodSubmission",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPremier",
                table: "DodSubmission",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChampionship",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "IsPremier",
                table: "DodSubmission");
        }
    }
}
