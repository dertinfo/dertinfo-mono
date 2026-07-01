using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DanceScorePartsAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanceScoreParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    JudgeSlotId = table.Column<int>(nullable: false),
                    DanceScoreId = table.Column<int>(nullable: false),
                    ScoreGiven = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanceScoreParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanceScoreParts_DanceScores_DanceScoreId",
                        column: x => x.DanceScoreId,
                        principalTable: "DanceScores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DanceScoreParts_JudgeSlots_JudgeSlotId",
                        column: x => x.JudgeSlotId,
                        principalTable: "JudgeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanceScoreParts_DanceScoreId",
                table: "DanceScoreParts",
                column: "DanceScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceScoreParts_JudgeSlotId",
                table: "DanceScoreParts",
                column: "JudgeSlotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanceScoreParts");
        }
    }
}
