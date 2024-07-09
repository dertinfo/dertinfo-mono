using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class competitionjudgejoin3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompetitionJudges_Competitions_CompetitionId",
                table: "CompetitionJudges");

            migrationBuilder.DropForeignKey(
                name: "FK_CompetitionJudges_Judges_JudgeId",
                table: "CompetitionJudges");

            migrationBuilder.RenameIndex(
                name: "IX_CompetitionJudges_JudgeId",
                table: "CompetitionJudges",
                newName: "IX_JudgeId");

            migrationBuilder.RenameIndex(
                name: "IX_CompetitionJudges_CompetitionId",
                table: "CompetitionJudges",
                newName: "IX_CompetitionId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateModified",
                table: "CompetitionJudges",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "CompetitionJudges",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.CompetitionJudges_dbo.Competitions_CompetitionId",
                table: "CompetitionJudges",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.CompetitionJudges_dbo.Judges_JudgeId",
                table: "CompetitionJudges",
                column: "JudgeId",
                principalTable: "Judges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.CompetitionJudges_dbo.Competitions_CompetitionId",
                table: "CompetitionJudges");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.CompetitionJudges_dbo.Judges_JudgeId",
                table: "CompetitionJudges");

            migrationBuilder.RenameIndex(
                name: "IX_JudgeId",
                table: "CompetitionJudges",
                newName: "IX_CompetitionJudges_JudgeId");

            migrationBuilder.RenameIndex(
                name: "IX_CompetitionId",
                table: "CompetitionJudges",
                newName: "IX_CompetitionJudges_CompetitionId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateModified",
                table: "CompetitionJudges",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "CompetitionJudges",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionJudges_Competitions_CompetitionId",
                table: "CompetitionJudges",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionJudges_Judges_JudgeId",
                table: "CompetitionJudges",
                column: "JudgeId",
                principalTable: "Judges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
