using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateTblSalaryUserVersion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UserCode",
            table: "SalaryUserVersions",
            newName: "Code");

        migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "SalaryUserVersions",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserId",
            table: "SalaryUserVersions");

        migrationBuilder.RenameColumn(
            name: "Code",
            table: "SalaryUserVersions",
            newName: "UserCode");
    }
}
