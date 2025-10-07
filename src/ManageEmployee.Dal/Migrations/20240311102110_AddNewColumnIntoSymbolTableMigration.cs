using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddNewColumnIntoSymbolTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CheckInTimeThreshold",
            table: "Symbols",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "CheckOutTimeThreshold",
            table: "Symbols",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CheckInTimeThreshold",
            table: "Symbols");

        migrationBuilder.DropColumn(
            name: "CheckOutTimeThreshold",
            table: "Symbols");
    }
}
