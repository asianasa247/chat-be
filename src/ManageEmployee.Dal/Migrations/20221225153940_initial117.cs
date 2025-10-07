using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial117 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Order",
            table: "Users");

        migrationBuilder.AddColumn<int>(
            name: "UserCreated",
            table: "Bills",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserCreated",
            table: "Bills");

        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
