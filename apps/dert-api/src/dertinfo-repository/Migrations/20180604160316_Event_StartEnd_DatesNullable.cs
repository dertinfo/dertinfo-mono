using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DertInfo.Repository.Migrations
{
    public partial class Event_StartEnd_DatesNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EventStartDate",
                table: "Events",
                type: "datetime",
                nullable: true,
                defaultValueSql: "'2015-04-10T00:00:00.000'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "'2015-04-10T00:00:00.000'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventEndDate",
                table: "Events",
                type: "datetime",
                nullable: true,
                defaultValueSql: "'2015-04-12T00:00:00.000'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "'2015-04-12T00:00:00.000'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EventStartDate",
                table: "Events",
                type: "datetime",
                nullable: false,
                defaultValueSql: "'2015-04-10T00:00:00.000'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "'2015-04-10T00:00:00.000'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventEndDate",
                table: "Events",
                type: "datetime",
                nullable: false,
                defaultValueSql: "'2015-04-12T00:00:00.000'",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "'2015-04-12T00:00:00.000'");
        }
    }
}
