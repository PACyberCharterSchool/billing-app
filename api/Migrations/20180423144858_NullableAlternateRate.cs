using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace api.Migrations
{
    public partial class NullableAlternateRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AlternateRate",
                table: "SchoolDistricts",
                nullable: true,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AlternateRate",
                table: "SchoolDistricts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
