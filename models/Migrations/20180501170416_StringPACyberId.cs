using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class StringPACyberId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_PACyberId",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "PACyberId",
                table: "Students",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "PACyberId",
                table: "StudentActivityRecords",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "PendingStudentStatusRecords",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "CommittedStudentStatusRecords",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Students_PACyberId",
                table: "Students",
                column: "PACyberId",
                unique: true,
                filter: "[PACyberId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_PACyberId",
                table: "Students");

            migrationBuilder.AlterColumn<int>(
                name: "PACyberId",
                table: "Students",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PACyberId",
                table: "StudentActivityRecords",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "PendingStudentStatusRecords",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "CommittedStudentStatusRecords",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_PACyberId",
                table: "Students",
                column: "PACyberId",
                unique: true);
        }
    }
}
