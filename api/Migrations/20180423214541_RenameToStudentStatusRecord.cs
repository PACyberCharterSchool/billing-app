using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class RenameToStudentStatusRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingStudentActivityRecords");

            migrationBuilder.CreateTable(
                name: "PendingStudentStatusRecords",
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
                    table.PrimaryKey("PK_PendingStudentStatusRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingStudentStatusRecords");

            migrationBuilder.CreateTable(
                name: "PendingStudentActivityRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivitySchoolYear = table.Column<string>(nullable: true),
                    SchoolDistrictId = table.Column<int>(nullable: false),
                    SchoolDistrictName = table.Column<string>(nullable: true),
                    StudentCity = table.Column<string>(nullable: true),
                    StudentCurrentIep = table.Column<DateTime>(nullable: true),
                    StudentDateOfBirth = table.Column<DateTime>(nullable: false),
                    StudentEnrollmentDate = table.Column<DateTime>(nullable: false),
                    StudentFirstName = table.Column<string>(nullable: true),
                    StudentFormerIep = table.Column<DateTime>(nullable: true),
                    StudentGradeLevel = table.Column<string>(nullable: true),
                    StudentId = table.Column<int>(nullable: false),
                    StudentIsSpecialEducation = table.Column<bool>(nullable: false),
                    StudentLastName = table.Column<string>(nullable: true),
                    StudentMiddleInitial = table.Column<string>(nullable: true),
                    StudentNorep = table.Column<DateTime>(nullable: true),
                    StudentPaSecuredId = table.Column<decimal>(nullable: true),
                    StudentState = table.Column<string>(nullable: true),
                    StudentStreet1 = table.Column<string>(nullable: true),
                    StudentStreet2 = table.Column<string>(nullable: true),
                    StudentWithdrawalDate = table.Column<DateTime>(nullable: true),
                    StudentZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingStudentActivityRecords", x => x.Id);
                });
        }
    }
}
