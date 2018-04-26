using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class StudentStartEndDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Students",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Students");
        }
    }
}
