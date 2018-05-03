using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace models.Migrations
{
    public partial class ActivitySequences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "StudentActivityRecords",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "StudentActivityRecords");
        }
    }
}
