using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertOfDertsUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeBuzzScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeCharactersScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeDanceTechniqueScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeMusicScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CumulativeNumberOfResults",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativePresentationScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeSteppingScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeSwordHandlingScore",
                table: "DodSubmission",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CumulativeBuzzScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeCharactersScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeDanceTechniqueScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeMusicScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeNumberOfResults",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativePresentationScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeSteppingScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeSwordHandlingScore",
                table: "DodSubmission");
        }
    }
}
