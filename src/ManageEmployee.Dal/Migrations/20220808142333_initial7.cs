using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial7 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            defaultValue: "",
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            defaultValue: "",
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            defaultValue: "",
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            defaultValue: "",
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Warehouses",
            type: "int",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Warehouses",
            type: "int",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Users",
            type: "int",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Users",
            type: "int",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }
}
