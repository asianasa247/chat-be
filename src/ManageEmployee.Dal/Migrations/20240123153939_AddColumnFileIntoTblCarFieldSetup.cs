using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnFileIntoTblCarFieldSetup : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserId",
            table: "CarFieldSetups");

        migrationBuilder.DropColumn(
            name: "ValueDate",
            table: "CarFieldSetups");

        migrationBuilder.AlterColumn<string>(
            name: "Image",
            table: "News",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileLink",
            table: "CarFieldSetups",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UserIdString",
            table: "CarFieldSetups",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileLink",
            table: "CarFieldSetups");

        migrationBuilder.DropColumn(
            name: "UserIdString",
            table: "CarFieldSetups");

        migrationBuilder.AlterColumn<string>(
            name: "Image",
            table: "News",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(1000)",
            oldMaxLength: 1000,
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "CarFieldSetups",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "ValueDate",
            table: "CarFieldSetups",
            type: "datetime2",
            nullable: true);
    }
}
