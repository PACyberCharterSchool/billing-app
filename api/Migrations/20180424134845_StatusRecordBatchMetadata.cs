using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class StatusRecordBatchMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchFilename",
                table: "PendingStudentStatusRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BatchHash",
                table: "PendingStudentStatusRecords",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BatchTime",
                table: "PendingStudentStatusRecords",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchFilename",
                table: "PendingStudentStatusRecords");

            migrationBuilder.DropColumn(
                name: "BatchHash",
                table: "PendingStudentStatusRecords");

            migrationBuilder.DropColumn(
                name: "BatchTime",
                table: "PendingStudentStatusRecords");
        }
    }
}
