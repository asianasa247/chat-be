using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial4 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Currency",
            table: "Companies",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "DayType",
            table: "Companies",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "DecimalRate",
            table: "Companies",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "DecimalUnit",
            table: "Companies",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "Money",
            table: "Companies",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Quatity",
            table: "Companies",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "ThousandUnit",
            table: "Companies",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "UnitCost",
            table: "Companies",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Currency",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "DayType",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "DecimalRate",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "DecimalUnit",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "Money",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "Quatity",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "ThousandUnit",
            table: "Companies");

        migrationBuilder.DropColumn(
            name: "UnitCost",
            table: "Companies");
    }
}
