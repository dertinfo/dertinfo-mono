using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertOfDertsUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DodResult_DodUser_DodUserId1",
                table: "DodResult");

            migrationBuilder.DropIndex(
                name: "IX_DodResult_DodUserId1",
                table: "DodResult");

            migrationBuilder.DropColumn(
                name: "DodUserId1",
                table: "DodResult");

            migrationBuilder.AlterColumn<int>(
                name: "DodUserId",
                table: "DodResult",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DodResult_DodUserId",
                table: "DodResult",
                column: "DodUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DodResult_DodUser_DodUserId",
                table: "DodResult",
                column: "DodUserId",
                principalTable: "DodUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DodResult_DodUser_DodUserId",
                table: "DodResult");

            migrationBuilder.DropIndex(
                name: "IX_DodResult_DodUserId",
                table: "DodResult");

            migrationBuilder.AlterColumn<string>(
                name: "DodUserId",
                table: "DodResult",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "DodUserId1",
                table: "DodResult",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DodResult_DodUserId1",
                table: "DodResult",
                column: "DodUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DodResult_DodUser_DodUserId1",
                table: "DodResult",
                column: "DodUserId1",
                principalTable: "DodUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
