using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class AuditDeltas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Field",
                table: "AuditRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "AuditRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Next",
                table: "AuditRecords",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Previous",
                table: "AuditRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Field",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "Next",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "Previous",
                table: "AuditRecords");
        }
    }
}
