using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddFromToAllowanceUserMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "WorkingDaysFrom",
            table: "AllowanceUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "WorkingDaysTo",
            table: "AllowanceUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WorkingDaysFrom",
            table: "AllowanceUsers");

        migrationBuilder.DropColumn(
            name: "WorkingDaysTo",
            table: "AllowanceUsers");
    }
}
