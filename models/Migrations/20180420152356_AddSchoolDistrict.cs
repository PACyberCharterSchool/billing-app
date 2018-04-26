using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace models.Migrations
{
	public partial class AddSchoolDistrict : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
					name: "SchoolDistrictId",
					table: "Students",
					nullable: true,
					oldClrType: typeof(int));

			migrationBuilder.CreateTable(
					name: "SchoolDistricts",
					columns: table => new
					{
						Id = table.Column<int>(nullable: false)
									.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
						AlternateRate = table.Column<decimal>(nullable: false),
						Aun = table.Column<int>(nullable: false),
						Name = table.Column<string>(nullable: true),
						Rate = table.Column<decimal>(nullable: false)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_SchoolDistricts", x => x.Id);
					});

			migrationBuilder.CreateIndex(
					name: "IX_Students_SchoolDistrictId",
					table: "Students",
					column: "SchoolDistrictId");

			migrationBuilder.AddForeignKey(
					name: "FK_Students_SchoolDistricts_SchoolDistrictId",
					table: "Students",
					column: "SchoolDistrictId",
					principalTable: "SchoolDistricts",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
					name: "FK_Students_SchoolDistricts_SchoolDistrictId",
					table: "Students");

			migrationBuilder.DropTable(
					name: "SchoolDistricts");

			migrationBuilder.DropIndex(
					name: "IX_Students_SchoolDistrictId",
					table: "Students");

			migrationBuilder.AlterColumn<int>(
					name: "SchoolDistrictId",
					table: "Students",
					nullable: false,
					oldClrType: typeof(int),
					oldNullable: true);
		}
	}
}
