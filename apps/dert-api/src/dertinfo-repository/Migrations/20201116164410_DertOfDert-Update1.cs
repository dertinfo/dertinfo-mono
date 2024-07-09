using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertOfDertUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DodResult_DodSubmission_DodSubmissionId",
                table: "DodResult");

            migrationBuilder.DropIndex(
                name: "IX_DodResult_DodSubmissionId",
                table: "DodResult");

            migrationBuilder.DropColumn(
                name: "DodSubmissionId",
                table: "DodResult");

            migrationBuilder.RenameColumn(
                name: "GdpaGained",
                table: "DodUser",
                newName: "TermsAndConditionsAgreed");

            migrationBuilder.RenameColumn(
                name: "DateGdpaGained",
                table: "DodUser",
                newName: "DateTermsAndConditionsAgreed");

            migrationBuilder.AddColumn<bool>(
                name: "IsOfficial",
                table: "DodUser",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfficial",
                table: "DodResult",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DodResult_SubmissionId",
                table: "DodResult",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DodResult_DodSubmission_SubmissionId",
                table: "DodResult",
                column: "SubmissionId",
                principalTable: "DodSubmission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DodResult_DodSubmission_SubmissionId",
                table: "DodResult");

            migrationBuilder.DropIndex(
                name: "IX_DodResult_SubmissionId",
                table: "DodResult");

            migrationBuilder.DropColumn(
                name: "IsOfficial",
                table: "DodUser");

            migrationBuilder.DropColumn(
                name: "IsOfficial",
                table: "DodResult");

            migrationBuilder.RenameColumn(
                name: "TermsAndConditionsAgreed",
                table: "DodUser",
                newName: "GdpaGained");

            migrationBuilder.RenameColumn(
                name: "DateTermsAndConditionsAgreed",
                table: "DodUser",
                newName: "DateGdpaGained");

            migrationBuilder.AddColumn<int>(
                name: "DodSubmissionId",
                table: "DodResult",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DodResult_DodSubmissionId",
                table: "DodResult",
                column: "DodSubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DodResult_DodSubmission_DodSubmissionId",
                table: "DodResult",
                column: "DodSubmissionId",
                principalTable: "DodSubmission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
