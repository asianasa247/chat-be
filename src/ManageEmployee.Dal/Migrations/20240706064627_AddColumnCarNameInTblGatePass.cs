using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCarNameInTblGatePass : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CarId",
            table: "GatePasses");

        migrationBuilder.AddColumn<string>(
            name: "CarName",
            table: "GatePasses",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CarName",
            table: "GatePasses");

        migrationBuilder.AddColumn<int>(
            name: "CarId",
            table: "GatePasses",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
