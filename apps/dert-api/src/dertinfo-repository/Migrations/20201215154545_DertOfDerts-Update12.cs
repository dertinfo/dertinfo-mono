using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertOfDertsUpdate12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YouTubeLink",
                table: "DodSubmission",
                newName: "EmbedOrigin");

            migrationBuilder.AddColumn<string>(
                name: "EmbedLink",
                table: "DodSubmission",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbedLink",
                table: "DodSubmission");

            migrationBuilder.RenameColumn(
                name: "EmbedOrigin",
                table: "DodSubmission",
                newName: "YouTubeLink");
        }
    }
}
