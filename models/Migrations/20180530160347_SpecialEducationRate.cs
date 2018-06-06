using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class SpecialEducationRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SpecialEducationRate",
                table: "SchoolDistricts",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialEducationRate",
                table: "SchoolDistricts");
        }
    }
}
