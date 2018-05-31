using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class AlternateSpecialEducationRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AlternateSpecialEducationRate",
                table: "SchoolDistricts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateSpecialEducationRate",
                table: "SchoolDistricts");
        }
    }
}
