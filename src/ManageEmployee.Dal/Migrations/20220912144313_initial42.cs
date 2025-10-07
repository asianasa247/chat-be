using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial42 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "VoucherNumber",
            table: "FixedAssets",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "UserManager",
            table: "FixedAssets",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "InvoiceTaxCode",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "InvoiceSerial",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "InvoiceNumber",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DepartmentManager",
            table: "FixedAssets",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitWarehouse",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecond",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirst",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditWarehouse",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecond",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirst",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "VoucherNumber",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserManager",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "InvoiceTaxCode",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "InvoiceSerial",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "InvoiceNumber",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DepartmentManager",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitWarehouse",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecond",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirst",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditWarehouse",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecond",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirst",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "FixedAssets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);
    }
}
