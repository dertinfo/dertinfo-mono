using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DertInfo.Repository.Migrations
{
    public partial class Event_Additional_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventTemplateType",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EventVisibilityType",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocationPostcode",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationTown",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTemplateType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventVisibilityType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationPostcode",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationTown",
                table: "Events");
        }
    }
}
