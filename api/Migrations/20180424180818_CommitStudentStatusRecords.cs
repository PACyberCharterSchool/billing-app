using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class CommitStudentStatusRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommittedStudentStatusRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivitySchoolYear = table.Column<string>(nullable: true),
                    BatchFilename = table.Column<string>(nullable: true),
                    BatchHash = table.Column<string>(nullable: true),
                    BatchTime = table.Column<DateTime>(nullable: false),
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
                    StudentZipCode = table.Column<string>(nullable: true),
                    CommitTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedStudentStatusRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommittedStudentStatusRecords");
        }
    }
}
