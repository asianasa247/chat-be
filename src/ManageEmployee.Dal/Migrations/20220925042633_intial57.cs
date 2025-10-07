using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial57 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MenuCode",
            table: "MenuRoles");

        migrationBuilder.DropColumn(
            name: "UserRoleCode",
            table: "MenuRoles");

        migrationBuilder.AddColumn<int>(
            name: "MenuId",
            table: "MenuRoles",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserRoleId",
            table: "MenuRoles",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MenuId",
            table: "MenuRoles");

        migrationBuilder.DropColumn(
            name: "UserRoleId",
            table: "MenuRoles");

        migrationBuilder.AddColumn<string>(
            name: "MenuCode",
            table: "MenuRoles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UserRoleCode",
            table: "MenuRoles",
            type: "nvarchar(max)",
            nullable: true);
    }
}
