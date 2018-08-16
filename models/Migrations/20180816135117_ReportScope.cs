using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class ReportScope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentRecords_StudentRecordsHeaders_HeaderId",
                table: "StudentRecords");

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Reports",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Reports");

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
