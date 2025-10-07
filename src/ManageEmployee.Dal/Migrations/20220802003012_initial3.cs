using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial3 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Role",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "UserRoleId",
            table: "Users");

        migrationBuilder.AddColumn<string>(
            name: "UserRoleIds",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserRoleIds",
            table: "Users");

        migrationBuilder.AddColumn<string>(
            name: "Role",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "UserRoleId",
            table: "Users",
            type: "int",
            nullable: true);
    }
}
