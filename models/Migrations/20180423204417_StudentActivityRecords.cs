using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
	public partial class StudentActivityRecords : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
					name: "PASecuredId",
					table: "Students",
					nullable: true,
					oldClrType: typeof(int));

			migrationBuilder.AlterColumn<DateTime>(
					name: "NOREP",
					table: "Students",
					nullable: true,
					oldClrType: typeof(DateTime));

			migrationBuilder.AlterColumn<string>(
					name: "Grade",
					table: "Students",
					nullable: true,
					oldClrType: typeof(int));

			migrationBuilder.AlterColumn<DateTime>(
					name: "FormerIep",
					table: "Students",
					nullable: true,
					oldClrType: typeof(DateTime));

			migrationBuilder.AlterColumn<DateTime>(
					name: "CurrentIep",
					table: "Students",
					nullable: true,
					oldClrType: typeof(DateTime));

			migrationBuilder.CreateTable(
					name: "StudentActivityRecords",
					columns: table => new
					{
						Id = table.Column<int>(nullable: false)
									.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
						SchoolDistrictId = table.Column<int>(nullable: false),
						SchoolDistrictName = table.Column<string>(nullable: true),
						StudentId = table.Column<int>(nullable: false),
						StudentFirstName = table.Column<string>(nullable: true),
						StudentMiddleInitial = table.Column<string>(nullable: true),
						StudentLastName = table.Column<string>(nullable: true),
						StudentGradeLevel = table.Column<string>(nullable: true),
						StudentDateOfBirth = table.Column<DateTime>(nullable: false),
						StudentStreet1 = table.Column<string>(nullable: true),
						StudentStreet2 = table.Column<string>(nullable: true),
						StudentCity = table.Column<string>(nullable: true),
						StudentState = table.Column<string>(nullable: true),
						StudentZipCode = table.Column<string>(nullable: true),
						ActivitySchoolYear = table.Column<string>(nullable: true),
						StudentEnrollmentDate = table.Column<DateTime>(nullable: false),
						StudentWithdrawalDate = table.Column<DateTime>(nullable: true),
						StudentIsSpecialEducation = table.Column<bool>(nullable: false),
						StudentCurrentIep = table.Column<DateTime>(nullable: true),
						StudentFormerIep = table.Column<DateTime>(nullable: true),
						StudentNorep = table.Column<DateTime>(nullable: true),
						StudentPaSecuredId = table.Column<decimal>(nullable: true)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_StudentActivityRecords", x => x.Id);
					});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
					name: "StudentActivityRecords");

			migrationBuilder.AlterColumn<int>(
					name: "PASecuredId",
					table: "Students",
					nullable: false,
					oldClrType: typeof(decimal),
					oldNullable: true);

			migrationBuilder.AlterColumn<DateTime>(
					name: "NOREP",
					table: "Students",
					nullable: false,
					oldClrType: typeof(DateTime),
					oldNullable: true);

			migrationBuilder.AlterColumn<int>(
					name: "Grade",
					table: "Students",
					nullable: false,
					oldClrType: typeof(string),
					oldNullable: true);

			migrationBuilder.AlterColumn<DateTime>(
					name: "FormerIep",
					table: "Students",
					nullable: false,
					oldClrType: typeof(DateTime),
					oldNullable: true);

			migrationBuilder.AlterColumn<DateTime>(
					name: "CurrentIep",
					table: "Students",
					nullable: false,
					oldClrType: typeof(DateTime),
					oldNullable: true);
		}
	}
}
