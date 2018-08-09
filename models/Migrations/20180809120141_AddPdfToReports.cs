using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class AddPdfToReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "DigitalSignatures",
                newName: "UserName");

            migrationBuilder.AddColumn<byte[]>(
                name: "Pdf",
                table: "Reports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pdf",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "DigitalSignatures",
                newName: "Username");
        }
    }
}
