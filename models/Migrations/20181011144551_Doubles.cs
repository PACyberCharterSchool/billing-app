using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class Doubles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SpecialEducationRate",
                table: "SchoolDistricts",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Rate",
                table: "SchoolDistricts",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "SchoolDistricts",
                nullable: true,
                defaultValue: "UniPay",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "Check");

            migrationBuilder.AlterColumn<double>(
                name: "AlternateSpecialEducationRate",
                table: "SchoolDistricts",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AlternateRate",
                table: "SchoolDistricts",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Refunds",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Payments",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SpecialEducationRate",
                table: "SchoolDistricts",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "SchoolDistricts",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "SchoolDistricts",
                nullable: true,
                defaultValue: "Check",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "UniPay");

            migrationBuilder.AlterColumn<decimal>(
                name: "AlternateSpecialEducationRate",
                table: "SchoolDistricts",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AlternateRate",
                table: "SchoolDistricts",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Refunds",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
