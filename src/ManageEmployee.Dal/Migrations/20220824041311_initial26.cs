using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial26 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Code",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "UserCode",
            table: "InOutHistories");

        migrationBuilder.AlterColumn<int>(
            name: "UnionMember",
            table: "Users",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "Timekeeper",
            table: "Users",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<double>(
            name: "SocialInsuranceSalary",
            table: "Users",
            type: "float",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<string>(
            name: "SocialInsuranceCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "ShareHolderCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<double>(
            name: "Salary",
            table: "Users",
            type: "float",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<string>(
            name: "Religion",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "PlaceOfPermanent",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Users",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20);

        migrationBuilder.AlterColumn<string>(
            name: "PersonalTaxCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<byte[]>(
            name: "PasswordSalt",
            table: "Users",
            type: "varbinary(max)",
            nullable: true,
            oldClrType: typeof(byte[]),
            oldType: "varbinary(max)");

        migrationBuilder.AlterColumn<byte[]>(
            name: "PasswordHash",
            table: "Users",
            type: "varbinary(max)",
            nullable: true,
            oldClrType: typeof(byte[]),
            oldType: "varbinary(max)");

        migrationBuilder.AlterColumn<double>(
            name: "NumberWorkdays",
            table: "Users",
            type: "float",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Nation",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "LiteracyDetail",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Literacy",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "LicensePlates",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Language",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "IdentifyCreatedPlace",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Identify",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Facebook",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "EthnicGroup",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "CertificateOther",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "BankAccount",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Bank",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Address",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UnionMember",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Timekeeper",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "SocialInsuranceSalary",
            table: "Users",
            type: "float",
            nullable: false,
            defaultValue: 0.0,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "SocialInsuranceCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ShareHolderCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Salary",
            table: "Users",
            type: "float",
            nullable: false,
            defaultValue: 0.0,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Religion",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PlaceOfPermanent",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Users",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PersonalTaxCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<byte[]>(
            name: "PasswordSalt",
            table: "Users",
            type: "varbinary(max)",
            nullable: false,
            defaultValue: new byte[0],
            oldClrType: typeof(byte[]),
            oldType: "varbinary(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<byte[]>(
            name: "PasswordHash",
            table: "Users",
            type: "varbinary(max)",
            nullable: false,
            defaultValue: new byte[0],
            oldClrType: typeof(byte[]),
            oldType: "varbinary(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "NumberWorkdays",
            table: "Users",
            type: "float",
            nullable: false,
            defaultValue: 0.0,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Nation",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "LiteracyDetail",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Literacy",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "LicensePlates",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Language",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "IdentifyCreatedPlace",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Identify",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Facebook",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "EthnicGroup",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CertificateOther",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "BankAccount",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Bank",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Address",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "UserCode",
            table: "InOutHistories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "");
    }
}
