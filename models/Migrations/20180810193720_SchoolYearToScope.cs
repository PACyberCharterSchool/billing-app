using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class SchoolYearToScope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentRecords_StudentRecordsHeaders_HeaderId",
                table: "StudentRecords");

            migrationBuilder.RenameColumn(
                name: "SchoolYear",
                table: "Reports",
                newName: "Scope");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentRecords_StudentRecordsHeaders_HeaderId",
                table: "StudentRecords",
                column: "HeaderId",
                principalTable: "StudentRecordsHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentRecords_StudentRecordsHeaders_HeaderId",
                table: "StudentRecords");

            migrationBuilder.RenameColumn(
                name: "Scope",
                table: "Reports",
                newName: "SchoolYear");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentRecords_StudentRecordsHeaders_HeaderId",
                table: "StudentRecords",
                column: "HeaderId",
                principalTable: "StudentRecordsHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
