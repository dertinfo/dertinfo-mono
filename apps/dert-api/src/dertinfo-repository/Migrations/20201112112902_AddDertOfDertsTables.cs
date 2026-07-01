using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class AddDertOfDertsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DodSubmission",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    YouTubeLink = table.Column<string>(nullable: true),
                    DertYearFrom = table.Column<string>(nullable: true),
                    DertVenueFrom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DodSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DodSubmission_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DodUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    GdpaGained = table.Column<bool>(nullable: false),
                    DateGdpaGained = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DodUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DodResult",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SubmissionId = table.Column<int>(nullable: false),
                    DodUserId = table.Column<string>(nullable: true),
                    MusicScore = table.Column<decimal>(nullable: false),
                    MusicComments = table.Column<string>(nullable: true),
                    SteppingScore = table.Column<decimal>(nullable: false),
                    SteppingComments = table.Column<string>(nullable: true),
                    SwordHandlingScore = table.Column<decimal>(nullable: false),
                    SwordHandlingComments = table.Column<string>(nullable: true),
                    DanceTechniqueScore = table.Column<decimal>(nullable: false),
                    DanceTechniqueComments = table.Column<string>(nullable: true),
                    PresentationScore = table.Column<decimal>(nullable: false),
                    PresentationComments = table.Column<string>(nullable: true),
                    BuzzScore = table.Column<decimal>(nullable: false),
                    BuzzComments = table.Column<string>(nullable: true),
                    CharactersScore = table.Column<decimal>(nullable: false),
                    CharactersComments = table.Column<string>(nullable: true),
                    OverallComments = table.Column<string>(nullable: true),
                    DodSubmissionId = table.Column<int>(nullable: true),
                    DodUserId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DodResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DodResult_DodSubmission_DodSubmissionId",
                        column: x => x.DodSubmissionId,
                        principalTable: "DodSubmission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DodResult_DodUser_DodUserId1",
                        column: x => x.DodUserId1,
                        principalTable: "DodUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DodResult_DodSubmissionId",
                table: "DodResult",
                column: "DodSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_DodResult_DodUserId1",
                table: "DodResult",
                column: "DodUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_DodSubmission_GroupId",
                table: "DodSubmission",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DodResult");

            migrationBuilder.DropTable(
                name: "DodSubmission");

            migrationBuilder.DropTable(
                name: "DodUser");
        }
    }
}
