using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DertInfo.Repository.Migrations
{
    public partial class Undo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TeamImages");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "TeamImages");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "TeamImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TeamImages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TeamImages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GroupImages");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "GroupImages");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "GroupImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GroupImages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "GroupImages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BreadcrumbItems");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "BreadcrumbItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BreadcrumbItems");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "BreadcrumbItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TeamImages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "TeamImages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "TeamImages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TeamImages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TeamImages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GroupImages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "GroupImages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "GroupImages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GroupImages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "GroupImages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "EventImages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "EventImages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "EventImages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EventImages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "EventImages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BreadcrumbItems",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "BreadcrumbItems",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BreadcrumbItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "BreadcrumbItems",
                nullable: true);
        }
    }
}
