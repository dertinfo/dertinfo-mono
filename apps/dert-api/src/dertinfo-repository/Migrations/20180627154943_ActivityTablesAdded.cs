using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DertInfo.Repository.Migrations
{
    public partial class ActivityTablesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EventStartDate",
                table: "Events",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "'2015-04-10T00:00:00.000'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventEndDate",
                table: "Events",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "'2015-04-12T00:00:00.000'");

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompetitionId = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EventId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityMemberAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MemberAttendanceId = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityMemberAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityMemberAttendances_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityMemberAttendances_MemberAttendances_MemberAttendanceId",
                        column: x => x.MemberAttendanceId,
                        principalTable: "MemberAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction); // DH - Edit was cacade but will not migrate
                });

            migrationBuilder.CreateTable(
                name: "ActivityTeamAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TeamAttendanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTeamAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTeamAttendances_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityTeamAttendances_TeamAttendances_TeamAttendanceId",
                        column: x => x.TeamAttendanceId,
                        principalTable: "TeamAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction); // DH - Edit was cacade but will not migrate
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CompetitionId",
                table: "Activities",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_EventId",
                table: "Activities",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityMemberAttendances_ActivityId",
                table: "ActivityMemberAttendances",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityMemberAttendances_MemberAttendanceId",
                table: "ActivityMemberAttendances",
                column: "MemberAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTeamAttendances_ActivityId",
                table: "ActivityTeamAttendances",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTeamAttendances_TeamAttendanceId",
                table: "ActivityTeamAttendances",
                column: "TeamAttendanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityMemberAttendances");

            migrationBuilder.DropTable(
                name: "ActivityTeamAttendances");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventStartDate",
                table: "Events",
                type: "datetime",
                nullable: true,
                defaultValueSql: "'2015-04-10T00:00:00.000'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventEndDate",
                table: "Events",
                type: "datetime",
                nullable: true,
                defaultValueSql: "'2015-04-12T00:00:00.000'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
