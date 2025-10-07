using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnSendNotificationInTblProcedureLog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsSendNotification",
            table: "ProcedureLogs",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "NotificationContent",
            table: "ProcedureLogs",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RoleIds",
            table: "ProcedureLogs",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UserIds",
            table: "ProcedureLogs",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsSendNotification",
            table: "ProcedureLogs");

        migrationBuilder.DropColumn(
            name: "NotificationContent",
            table: "ProcedureLogs");

        migrationBuilder.DropColumn(
            name: "RoleIds",
            table: "ProcedureLogs");

        migrationBuilder.DropColumn(
            name: "UserIds",
            table: "ProcedureLogs");
    }
}
