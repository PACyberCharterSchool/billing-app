using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class DigitalSignatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.CreateTable(
              name: "DigitalSignatures",
              columns: table => new
              {
                Id = table.Column<int>(nullable: false)
                  .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                Title = table.Column<string>(nullable: false),
                FileName = table.Column<string>(nullable: false),
                Username = table.Column<string>(nullable: false),
                Created = table.Column<DateTime>(nullable: false),
                LastUpdated = table.Column<DateTime>(nullable: false)
              },
              constraints: table =>
              {
                table.PrimaryKey("PK_DigitalSignatures", x => x.Id);
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropTable(
              name: "DigitalSignatures"
              );
        }
    }
}
