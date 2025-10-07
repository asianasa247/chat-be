using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial49 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Introduces",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(200)",
            oldMaxLength: 200);

        migrationBuilder.AlterColumn<string>(
            name: "IframeYoutube",
            table: "Introduces",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(200)",
            oldMaxLength: 200);

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Introduces",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(1000)",
            oldMaxLength: 1000);

        migrationBuilder.AlterColumn<string>(
            name: "UserCode",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "TypePay",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Status",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Bills",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DiscountType",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CustomerName",
            table: "Bills",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "BillDetails",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DiscountType",
            table: "BillDetails",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Introduces",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(200)",
            oldMaxLength: 200,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "IframeYoutube",
            table: "Introduces",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(200)",
            oldMaxLength: 200,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Introduces",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(1000)",
            oldMaxLength: 1000,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCode",
            table: "Bills",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "TypePay",
            table: "Bills",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Status",
            table: "Bills",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Bills",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DiscountType",
            table: "Bills",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CustomerName",
            table: "Bills",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "BillDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DiscountType",
            table: "BillDetails",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);
    }
}
