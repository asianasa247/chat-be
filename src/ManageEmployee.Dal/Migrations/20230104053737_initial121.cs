using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial121 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAssetUser",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAssetUser",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAssetUser",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAssetUser",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAssetUser",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAssetUser",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);
    }
}
