using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class StudentActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NOREP",
                table: "Students",
                newName: "NorepDate");

            migrationBuilder.CreateTable(
                name: "StudentActivityRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PACyberId = table.Column<int>(nullable: false),
                    Activity = table.Column<string>(nullable: false),
                    PreviousData = table.Column<string>(nullable: true),
                    NextData = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    BatchHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentActivityRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_PACyberId",
                table: "Students",
                column: "PACyberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolDistricts_Aun",
                table: "SchoolDistricts",
                column: "Aun",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentActivityRecords");

            migrationBuilder.DropIndex(
                name: "IX_Students_PACyberId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_SchoolDistricts_Aun",
                table: "SchoolDistricts");

            migrationBuilder.RenameColumn(
                name: "NorepDate",
                table: "Students",
                newName: "NOREP");
        }
    }
}
