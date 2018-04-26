using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
	public partial class PendingStudentActivityRecords : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
					name: "PK_StudentActivityRecords",
					table: "StudentActivityRecords");

			migrationBuilder.RenameTable(
					name: "StudentActivityRecords",
					newName: "PendingStudentActivityRecords");

			migrationBuilder.AddPrimaryKey(
					name: "PK_PendingStudentActivityRecords",
					table: "PendingStudentActivityRecords",
					column: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
					name: "PK_PendingStudentActivityRecords",
					table: "PendingStudentActivityRecords");

			migrationBuilder.RenameTable(
					name: "PendingStudentActivityRecords",
					newName: "StudentActivityRecords");

			migrationBuilder.AddPrimaryKey(
					name: "PK_StudentActivityRecords",
					table: "StudentActivityRecords",
					column: "Id");
		}
	}
}
