using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInTblP_SalaryAdvance : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DeleteAt",
            table: "P_SalaryAdvance");

        migrationBuilder.DropColumn(
            name: "IsDelete",
            table: "P_SalaryAdvance");

        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "ProcedureChangeShifts",
            newName: "IsFinished");

        migrationBuilder.RenameColumn(
            name: "P_ProcedureStatusName",
            table: "P_SalaryAdvance",
            newName: "ProcedureStatusName");

        migrationBuilder.RenameColumn(
            name: "P_ProcedureStatusId",
            table: "P_SalaryAdvance",
            newName: "ProcedureStatusId");

        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "P_SalaryAdvance",
            newName: "IsFinished");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "ProcedureChangeShifts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "ProcedureChangeShifts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "ProcedureChangeShifts",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_SalaryAdvance",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "P_SalaryAdvance",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "P_SalaryAdvance",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Code",
            table: "ProcedureChangeShifts");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "ProcedureChangeShifts");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "P_SalaryAdvance");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "P_SalaryAdvance");

        migrationBuilder.RenameColumn(
            name: "IsFinished",
            table: "ProcedureChangeShifts",
            newName: "IsFinish");

        migrationBuilder.RenameColumn(
            name: "ProcedureStatusName",
            table: "P_SalaryAdvance",
            newName: "P_ProcedureStatusName");

        migrationBuilder.RenameColumn(
            name: "ProcedureStatusId",
            table: "P_SalaryAdvance",
            newName: "P_ProcedureStatusId");

        migrationBuilder.RenameColumn(
            name: "IsFinished",
            table: "P_SalaryAdvance",
            newName: "IsFinish");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "ProcedureChangeShifts",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_SalaryAdvance",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DeleteAt",
            table: "P_SalaryAdvance",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDelete",
            table: "P_SalaryAdvance",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }
}
