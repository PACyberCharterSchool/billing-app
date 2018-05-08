using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class BatchRow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "SchoolDistricts",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "ACH");

            migrationBuilder.AddColumn<int>(
                name: "BatchRow",
                table: "CommittedStudentStatusRecords",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchRow",
                table: "CommittedStudentStatusRecords");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "SchoolDistricts",
                nullable: true,
                defaultValue: "ACH",
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
