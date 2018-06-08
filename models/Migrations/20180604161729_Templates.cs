using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class Templates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReportType = table.Column<string>(nullable: true),
                    SchoolYear = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Content = table.Column<byte[]>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Templates_ReportType_SchoolYear",
                table: "Templates",
                columns: new[] { "ReportType", "SchoolYear" },
                unique: true,
                filter: "[ReportType] IS NOT NULL AND [SchoolYear] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
