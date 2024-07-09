using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class DertOfDertsUpdate51 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasOutstandingComplaint",
                table: "DodResult",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInScores",
                table: "DodResult",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "DodResultComplaint",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ResultId = table.Column<int>(nullable: false),
                    ForScores = table.Column<bool>(nullable: false),
                    ForComments = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    DodResultId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DodResultComplaint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DodResultComplaint_DodResult_DodResultId",
                        column: x => x.DodResultId,
                        principalTable: "DodResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DodResultComplaint_DodResultId",
                table: "DodResultComplaint",
                column: "DodResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DodResultComplaint");

            migrationBuilder.DropColumn(
                name: "HasOutstandingComplaint",
                table: "DodResult");

            migrationBuilder.DropColumn(
                name: "IncludeInScores",
                table: "DodResult");
        }
    }
}
