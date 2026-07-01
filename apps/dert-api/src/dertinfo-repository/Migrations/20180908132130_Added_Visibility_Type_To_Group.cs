using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class Added_Visibility_Type_To_Group : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupVisibilityType",
                table: "Groups",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupVisibilityType",
                table: "Groups");
        }
    }
}
