using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DertInfo.Repository.Migrations
{
    public partial class Event_Contact_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactTelephone",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ContactTelephone",
                table: "Events");
        }
    }
}
