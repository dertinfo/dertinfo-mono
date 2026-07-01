using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class NotificationsUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcknowledged",
                table: "NotificationAudienceLogs");

            migrationBuilder.DropColumn(
                name: "IsCleared",
                table: "NotificationAudienceLogs");

            migrationBuilder.RenameColumn(
                name: "IsDismissable",
                table: "NotificationMessages",
                newName: "RequiresOpening");

            migrationBuilder.RenameColumn(
                name: "DateDisplayedOn",
                table: "NotificationAudienceLogs",
                newName: "DateNotifiedOn");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresAcknowledgement",
                table: "NotificationMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDismissedOn",
                table: "NotificationAudienceLogs",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresAcknowledgement",
                table: "NotificationMessages");

            migrationBuilder.DropColumn(
                name: "DateDismissedOn",
                table: "NotificationAudienceLogs");

            migrationBuilder.RenameColumn(
                name: "RequiresOpening",
                table: "NotificationMessages",
                newName: "IsDismissable");

            migrationBuilder.RenameColumn(
                name: "DateNotifiedOn",
                table: "NotificationAudienceLogs",
                newName: "DateDisplayedOn");

            migrationBuilder.AddColumn<bool>(
                name: "IsAcknowledged",
                table: "NotificationAudienceLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCleared",
                table: "NotificationAudienceLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
