using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace models.Migrations
{
	public partial class ExpandStudent : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
					name: "lastName",
					table: "Students",
					newName: "LastName");

			migrationBuilder.RenameColumn(
					name: "firstName",
					table: "Students",
					newName: "FirstName");

			migrationBuilder.AddColumn<string>(
					name: "City",
					table: "Students",
					nullable: true);

			migrationBuilder.AddColumn<DateTime>(
					name: "CurrentIep",
					table: "Students",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
					name: "DateOfBirth",
					table: "Students",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<DateTime>(
					name: "FormerIep",
					table: "Students",
					nullable: false,
					defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

			migrationBuilder.AddColumn<int>(
					name: "Grade",
					table: "Students",
					nullable: false,
					defaultValue: 0);

			migrationBuilder.AddColumn<bool>(
					name: "IsSpecialEducation",
					table: "Students",
					nullable: false,
					defaultValue: false);

			migrationBuilder.AddColumn<string>(
					name: "MiddleInitial",
					table: "Students",
					nullable: true);

			migrationBuilder.AddColumn<int>(
					name: "PACyberId",
					table: "Students",
					nullable: false,
					defaultValue: 0);

			migrationBuilder.AddColumn<int>(
					name: "PASecuredId",
					table: "Students",
					nullable: false,
					defaultValue: 0);

			migrationBuilder.AddColumn<int>(
					name: "SchoolDistrictId",
					table: "Students",
					nullable: false,
					defaultValue: 0);

			migrationBuilder.AddColumn<string>(
					name: "State",
					table: "Students",
					nullable: true);

			migrationBuilder.AddColumn<string>(
					name: "Street1",
					table: "Students",
					nullable: true);

			migrationBuilder.AddColumn<string>(
					name: "Street2",
					table: "Students",
					nullable: true);

			migrationBuilder.AddColumn<string>(
					name: "ZipCode",
					table: "Students",
					nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
					name: "City",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "CurrentIep",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "DateOfBirth",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "FormerIep",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "Grade",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "IsSpecialEducation",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "MiddleInitial",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "PACyberId",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "PASecuredId",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "SchoolDistrictId",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "State",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "Street1",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "Street2",
					table: "Students");

			migrationBuilder.DropColumn(
					name: "ZipCode",
					table: "Students");

			migrationBuilder.RenameColumn(
					name: "LastName",
					table: "Students",
					newName: "lastName");

			migrationBuilder.RenameColumn(
					name: "FirstName",
					table: "Students",
					newName: "firstName");
		}
	}
}
