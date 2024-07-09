using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertOfDertsUpdate6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DodResultComplaint_DodResult_DodResultId",
                table: "DodResultComplaint");

            migrationBuilder.DropColumn(
                name: "ResultId",
                table: "DodResultComplaint");

            migrationBuilder.AlterColumn<int>(
                name: "DodResultId",
                table: "DodResultComplaint",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DodResultComplaint_DodResult_DodResultId",
                table: "DodResultComplaint",
                column: "DodResultId",
                principalTable: "DodResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DodResultComplaint_DodResult_DodResultId",
                table: "DodResultComplaint");

            migrationBuilder.AlterColumn<int>(
                name: "DodResultId",
                table: "DodResultComplaint",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ResultId",
                table: "DodResultComplaint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_DodResultComplaint_DodResult_DodResultId",
                table: "DodResultComplaint",
                column: "DodResultId",
                principalTable: "DodResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
