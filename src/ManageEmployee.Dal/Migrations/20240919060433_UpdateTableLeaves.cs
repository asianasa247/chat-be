using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateTableLeaves : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DeleteAt",
            table: "P_Leave");

        migrationBuilder.DropColumn(
            name: "IsDelete",
            table: "P_Leave");

        migrationBuilder.RenameColumn(
            name: "P_ProcedureStatusName",
            table: "P_Leave",
            newName: "ProcedureStatusName");

        migrationBuilder.RenameColumn(
            name: "P_ProcedureStatusId",
            table: "P_Leave",
            newName: "ProcedureStatusId");

        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "P_Leave",
            newName: "IsFinished");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Leave",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "P_Leave",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "P_Leave",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Code",
            table: "P_Leave");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "P_Leave");

        migrationBuilder.RenameColumn(
            name: "ProcedureStatusName",
            table: "P_Leave",
            newName: "P_ProcedureStatusName");

        migrationBuilder.RenameColumn(
            name: "ProcedureStatusId",
            table: "P_Leave",
            newName: "P_ProcedureStatusId");

        migrationBuilder.RenameColumn(
            name: "IsFinished",
            table: "P_Leave",
            newName: "IsFinish");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Leave",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DeleteAt",
            table: "P_Leave",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDelete",
            table: "P_Leave",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }
}
