using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class Reports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true),
                    SchoolYear = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Approved = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Xlsx = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Name",
                table: "Reports",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
