using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
	public partial class SchoolDistrictPaymentTypeMigration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
					name: "PaymentType",
					table: "SchoolDistricts",
					nullable: true,
					defaultValue: "ACH");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
					name: "PaymentType",
					table: "SchoolDistricts");
		}
	}
}
