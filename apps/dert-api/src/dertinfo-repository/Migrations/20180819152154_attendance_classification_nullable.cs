using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DertInfo.Repository.Migrations
{
    public partial class attendance_classification_nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.MemberAttendances_dbo.AttendanceClassifications_AttendanceClassificationId",
                table: "MemberAttendances");

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceClassificationId",
                table: "MemberAttendances",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.MemberAttendances_dbo.AttendanceClassifications_AttendanceClassificationId",
                table: "MemberAttendances",
                column: "AttendanceClassificationId",
                principalTable: "AttendanceClassifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dbo.MemberAttendances_dbo.AttendanceClassifications_AttendanceClassificationId",
                table: "MemberAttendances");

            migrationBuilder.AlterColumn<int>(
                name: "AttendanceClassificationId",
                table: "MemberAttendances",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.MemberAttendances_dbo.AttendanceClassifications_AttendanceClassificationId",
                table: "MemberAttendances",
                column: "AttendanceClassificationId",
                principalTable: "AttendanceClassifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
