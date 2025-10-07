using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnPassEmailIntoTblUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PassEmail",
            table: "Users",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "SignFile",
            table: "Users",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PassEmail",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "SignFile",
            table: "Users");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);
    }
}
