using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DertInfo.Repository.Migrations
{
    public partial class GroupOriginFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginPostcode",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginTown",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginPostcode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "OriginTown",
                table: "Groups");
        }
    }
}
