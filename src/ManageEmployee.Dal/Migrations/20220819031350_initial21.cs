using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial21 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserTasks",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "FileLink",
            table: "UserTasks",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "UserTasks",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserTaskCheckLists",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "FileLink",
            table: "UserTaskCheckLists",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FileLink",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserTaskCheckLists",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "FileLink",
            table: "UserTaskCheckLists",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);
    }
}
