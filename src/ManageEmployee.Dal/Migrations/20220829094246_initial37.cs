using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial37 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "BranchId",
            table: "Warehouses",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "BranchId",
            table: "Departments",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BranchId",
            table: "Warehouses");

        migrationBuilder.DropColumn(
            name: "BranchId",
            table: "Departments");
    }
}
