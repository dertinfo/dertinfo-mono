using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class NotificationsInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // note - we manually deleted this on live not realising that it was part of the new model. It was the old web notifications table
            //migrationBuilder.DropTable(
            //    name: "Notifications");

            migrationBuilder.CreateTable(
                name: "NotificationLastChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAuth0Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastCheckPerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasUnreadMessages = table.Column<bool>(type: "bit", nullable: false),
                    MaximumMessageSeverity = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLastChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageHeader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasDetails = table.Column<bool>(type: "bit", nullable: false),
                    IsDismissable = table.Column<bool>(type: "bit", nullable: false),
                    BlocksUser = table.Column<bool>(type: "bit", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationAudienceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationMessageId = table.Column<int>(type: "int", nullable: false),
                    UserAuth0Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDisplayedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateSeenOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOpenedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    DateAcknowledgedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCleared = table.Column<bool>(type: "bit", nullable: false),
                    DateClearedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationAudienceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationAudienceLogs_NotificationMessages_NotificationMessageId",
                        column: x => x.NotificationMessageId,
                        principalTable: "NotificationMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAudienceLogs_NotificationMessageId",
                table: "NotificationAudienceLogs",
                column: "NotificationMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationAudienceLogs");

            migrationBuilder.DropTable(
                name: "NotificationLastChecks");

            migrationBuilder.DropTable(
                name: "NotificationMessages");

            // note - we manually deleted this on live not realising that it was part of the new model. It was the old web notifications table
            //migrationBuilder.CreateTable(
            //    name: "Notifications",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
            //        DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //        Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserAppliesTo = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Notifications", x => x.Id);
            //    });
        }
    }
}
