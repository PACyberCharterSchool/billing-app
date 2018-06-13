using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class AddImageDataToDigitalSignatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "imgData",
                table: "DigitalSignatures",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imgData",
                table: "DigitalSignatures");
        }
    }
}
