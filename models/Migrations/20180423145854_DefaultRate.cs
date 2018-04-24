using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace models.Migrations
{
	public partial class DefaultRate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
					name: "Rate",
					table: "SchoolDistricts",
					nullable: false,
					defaultValue: 0m,
					oldClrType: typeof(decimal));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
					name: "Rate",
					table: "SchoolDistricts",
					nullable: false,
					oldClrType: typeof(decimal),
					oldDefaultValue: 0m);
		}
	}
}
