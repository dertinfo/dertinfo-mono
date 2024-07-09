using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class Terms_And_Conditions_Added_To_Group_And_Event : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TermsAndConditionsAgreed",
                table: "Groups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditionsAgreedBy",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TermsAndConditionsAgreed",
                table: "Events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditionsAgreedBy",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAndConditionsAgreed",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TermsAndConditionsAgreedBy",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TermsAndConditionsAgreed",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TermsAndConditionsAgreedBy",
                table: "Events");
        }
    }
}
