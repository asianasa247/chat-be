using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial62 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "BranchId",
            table: "TypeWorks",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Color",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "DepartmentId",
            table: "TypeWorks",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BranchId",
            table: "TypeWorks");

        migrationBuilder.DropColumn(
            name: "Color",
            table: "TypeWorks");

        migrationBuilder.DropColumn(
            name: "DepartmentId",
            table: "TypeWorks");
    }
}
