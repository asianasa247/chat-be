using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnFileNameInDocument : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "FileUrl",
            table: "DocumentType2",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileName",
            table: "DocumentType2",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FileUrl",
            table: "DocumentType1",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileName",
            table: "DocumentType1",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileName",
            table: "DocumentType2");

        migrationBuilder.DropColumn(
            name: "FileName",
            table: "DocumentType1");

        migrationBuilder.AlterColumn<string>(
            name: "FileUrl",
            table: "DocumentType2",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FileUrl",
            table: "DocumentType1",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
