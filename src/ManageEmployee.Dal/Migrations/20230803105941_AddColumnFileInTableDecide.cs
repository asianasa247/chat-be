using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnFileInTableDecide : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FileName",
            table: "HistoryAchievements",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileUrl",
            table: "HistoryAchievements",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FileUrl",
            table: "Decide",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileName",
            table: "Decide",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileName",
            table: "HistoryAchievements");

        migrationBuilder.DropColumn(
            name: "FileUrl",
            table: "HistoryAchievements");

        migrationBuilder.DropColumn(
            name: "FileName",
            table: "Decide");

        migrationBuilder.AlterColumn<string>(
            name: "FileUrl",
            table: "Decide",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);
    }
}
