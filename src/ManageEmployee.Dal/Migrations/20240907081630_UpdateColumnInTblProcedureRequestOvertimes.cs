using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInTblProcedureRequestOvertimes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "ProcedureRequestOvertimes",
            newName: "IsFinished");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "ProcedureRequestOvertimes",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "ProcedureRequestOvertimes",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "ProcedureRequestOvertimes",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Code",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.RenameColumn(
            name: "IsFinished",
            table: "ProcedureRequestOvertimes",
            newName: "IsFinish");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "ProcedureRequestOvertimes",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);
    }
}
