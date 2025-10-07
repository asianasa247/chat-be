using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial120 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Name",
            table: "MainColors");

        migrationBuilder.DropColumn(
            name: "Note",
            table: "MainColors");

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "MainColors",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AddColumn<bool>(
            name: "IsDark",
            table: "MainColors",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Theme",
            table: "MainColors",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "MainColors",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDark",
            table: "MainColors");

        migrationBuilder.DropColumn(
            name: "Theme",
            table: "MainColors");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "MainColors");

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "MainColors",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Name",
            table: "MainColors",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Note",
            table: "MainColors",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");
    }
}
