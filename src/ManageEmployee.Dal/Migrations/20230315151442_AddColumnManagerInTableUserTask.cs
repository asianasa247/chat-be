using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnManagerInTableUserTask : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "IsStatusForManager",
            table: "UserTasks",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "ManagerUpdateDate",
            table: "UserTasks",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserManagerId",
            table: "UserTasks",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsStatusForManager",
            table: "UserTasks");

        migrationBuilder.DropColumn(
            name: "ManagerUpdateDate",
            table: "UserTasks");

        migrationBuilder.DropColumn(
            name: "UserManagerId",
            table: "UserTasks");
    }
}
