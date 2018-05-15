using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class Rebuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: false),
                    Activity = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommittedStudentStatusRecords",
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
                    BatchTime = table.Column<DateTime>(nullable: false),
                    BatchFilename = table.Column<string>(nullable: true),
                    BatchHash = table.Column<string>(nullable: true),
                    CommitTime = table.Column<DateTime>(nullable: false),
                    BatchRow = table.Column<int>(nullable: false)
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
                    BatchTime = table.Column<DateTime>(nullable: false),
                    BatchFilename = table.Column<string>(nullable: true),
                    BatchHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingStudentStatusRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolDistricts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Aun = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rate = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    AlternateRate = table.Column<decimal>(nullable: true),
                    PaymentType = table.Column<string>(nullable: true, defaultValue: "ACH"),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolDistricts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentActivityRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PACyberId = table.Column<string>(nullable: true),
                    Activity = table.Column<string>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    PreviousData = table.Column<string>(nullable: true),
                    NextData = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    BatchHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentActivityRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentId = table.Column<string>(nullable: true),
                    Split = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true, defaultValue: "Check"),
                    Amount = table.Column<decimal>(nullable: false),
                    SchoolYear = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    SchoolDistrictId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_SchoolDistricts_SchoolDistrictId",
                        column: x => x.SchoolDistrictId,
                        principalTable: "SchoolDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PACyberId = table.Column<string>(nullable: true),
                    PASecuredId = table.Column<decimal>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleInitial = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Street1 = table.Column<string>(nullable: true),
                    Street2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    IsSpecialEducation = table.Column<bool>(nullable: false),
                    CurrentIep = table.Column<DateTime>(nullable: true),
                    FormerIep = table.Column<DateTime>(nullable: true),
                    NorepDate = table.Column<DateTime>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    SchoolDistrictId = table.Column<int>(nullable: true)
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
                name: "IX_Payments_SchoolDistrictId",
                table: "Payments",
                column: "SchoolDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentId_Split",
                table: "Payments",
                columns: new[] { "PaymentId", "Split" },
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolDistricts_Aun",
                table: "SchoolDistricts",
                column: "Aun",
                unique: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecords");

            migrationBuilder.DropTable(
                name: "CommittedStudentStatusRecords");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PendingStudentStatusRecords");

            migrationBuilder.DropTable(
                name: "StudentActivityRecords");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "SchoolDistricts");
        }
    }
}
