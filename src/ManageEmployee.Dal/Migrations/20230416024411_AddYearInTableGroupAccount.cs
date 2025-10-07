using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddYearInTableGroupAccount : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Year",
            table: "ChartOfAccountGroups",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Year",
            table: "ChartOfAccountGroupLinks",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Year",
            table: "ChartOfAccountGroups");

        migrationBuilder.DropColumn(
            name: "Year",
            table: "ChartOfAccountGroupLinks");
    }
}
