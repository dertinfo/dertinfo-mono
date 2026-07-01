using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DropCumulativeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "CumulativePresentationScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeSteppingScore",
                table: "DodSubmission");

            migrationBuilder.DropColumn(
                name: "CumulativeSwordHandlingScore",
                table: "DodSubmission");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeBuzzScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeCharactersScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeDanceTechniqueScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeMusicScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativePresentationScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeSteppingScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeSwordHandlingScore",
                table: "DodSubmission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
