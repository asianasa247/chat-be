using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateTblConfigurationUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_ConfigurationUsers",
            table: "ConfigurationUsers");

        migrationBuilder.AlterColumn<string>(
            name: "Key",
            table: "ConfigurationUsers",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);

        migrationBuilder.AddColumn<int>(
            name: "Id",
            table: "ConfigurationUsers",
            type: "int",
            nullable: false,
            defaultValue: 0)
            .Annotation("SqlServer:Identity", "1, 1");

        migrationBuilder.AddPrimaryKey(
            name: "PK_ConfigurationUsers",
            table: "ConfigurationUsers",
            column: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_ConfigurationUsers",
            table: "ConfigurationUsers");

        migrationBuilder.DropColumn(
            name: "Id",
            table: "ConfigurationUsers");

        migrationBuilder.AlterColumn<string>(
            name: "Key",
            table: "ConfigurationUsers",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100,
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_ConfigurationUsers",
            table: "ConfigurationUsers",
            column: "Key");
    }
}
