using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class RefactorStudentRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommittedStudentStatusRecords");

            migrationBuilder.DropTable(
                name: "PendingStudentStatusRecords");

            migrationBuilder.DropTable(
                name: "StudentActivityRecords");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "DigitalSignatures",
                newName: "UserName");

            migrationBuilder.CreateTable(
                name: "StudentRecordsHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Scope = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Locked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRecordsHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SchoolDistrictId = table.Column<int>(nullable: false),
                    SchoolDistrictName = table.Column<string>(nullable: true),
                    StudentId = table.Column<string>(nullable: true),
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
                    StudentPaSecuredId = table.Column<decimal>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    HeaderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRecords_StudentRecordsHeaders_HeaderId",
                        column: x => x.HeaderId,
                        principalTable: "StudentRecordsHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRecords_HeaderId",
                table: "StudentRecords",
                column: "HeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRecordsHeaders_Scope",
                table: "StudentRecordsHeaders",
                column: "Scope",
                unique: true,
                filter: "[Scope] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentRecords");

            migrationBuilder.DropTable(
                name: "StudentRecordsHeaders");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "DigitalSignatures",
                newName: "Username");

            migrationBuilder.CreateTable(
                name: "CommittedStudentStatusRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivitySchoolYear = table.Column<string>(nullable: true),
                    BatchFilename = table.Column<string>(nullable: true),
                    BatchHash = table.Column<string>(nullable: true),
                    BatchRow = table.Column<int>(nullable: false),
                    BatchTime = table.Column<DateTime>(nullable: false),
                    CommitTime = table.Column<DateTime>(nullable: false),
                    SchoolDistrictId = table.Column<int>(nullable: false),
                    SchoolDistrictName = table.Column<string>(nullable: true),
                    StudentCity = table.Column<string>(nullable: true),
                    StudentCurrentIep = table.Column<DateTime>(nullable: true),
                    StudentDateOfBirth = table.Column<DateTime>(nullable: false),
                    StudentEnrollmentDate = table.Column<DateTime>(nullable: false),
                    StudentFirstName = table.Column<string>(nullable: true),
                    StudentFormerIep = table.Column<DateTime>(nullable: true),
                    StudentGradeLevel = table.Column<string>(nullable: true),
                    StudentId = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_CommittedStudentStatusRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PendingStudentStatusRecords",
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
                    StudentId = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_PendingStudentStatusRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentActivityRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activity = table.Column<string>(nullable: false),
                    BatchHash = table.Column<string>(nullable: true),
                    NextData = table.Column<string>(nullable: true),
                    PACyberId = table.Column<string>(nullable: true),
                    PreviousData = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentActivityRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CurrentIep = table.Column<DateTime>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    FormerIep = table.Column<DateTime>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    IsSpecialEducation = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    MiddleInitial = table.Column<string>(nullable: true),
                    NorepDate = table.Column<DateTime>(nullable: true),
                    PACyberId = table.Column<string>(nullable: true),
                    PASecuredId = table.Column<decimal>(nullable: true),
                    SchoolDistrictId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Street1 = table.Column<string>(nullable: true),
                    Street2 = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_SchoolDistricts_SchoolDistrictId",
                        column: x => x.SchoolDistrictId,
                        principalTable: "SchoolDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_PACyberId",
                table: "Students",
                column: "PACyberId",
                unique: true,
                filter: "[PACyberId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolDistrictId",
                table: "Students",
                column: "SchoolDistrictId");
        }
    }
}
