using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
	public partial class CreatedLastUpdated : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
					name: "Created",
					table: "Students",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
					name: "LastUpdated",
					table: "Students",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
					name: "Created",
					table: "SchoolDistricts",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
					name: "LastUpdated",
					table: "SchoolDistricts",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
					name: "Created",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "LastUpdated",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "Created",
					table: "SchoolDistricts");

			migrationBuilder.DropColumn(
					name: "LastUpdated",
					table: "SchoolDistricts");
		}
	}
}
