using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddDataAccountPay : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AccountPays",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName2",
            table: "AccountPays",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1",
            table: "AccountPays",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "AccountPays",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.InsertData(
            table: "AccountPays",
            columns: new[] { "Id", "Account", "AccountName", "Code", "Detail1", "Detail2", "DetailName1", "DetailName2", "Name" },
            values: new object[,]
            {
                { 1, "1111", "Tiền Việt Nam", "TM", "TM", "", "Tiền mặt tại quỹ", "", "Tiền mặt" },
                { 2, "33311", "Thuế GTGT đầu ra", "VAT", "", "", "", "", "Thuế VAT" },
                { 3, "1121", "Tiền Việt Nam", "NH", "BIDV", "", "Ngân hàng BIDV", "", "Ngân hàng" },
                { 4, "1331", "Thuế GTGT được khấu trừ của hàng hóa, dịch vụ", "DV", "", "", "", "", "Thuế đầu vào" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AccountPays",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "AccountPays",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "AccountPays",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "AccountPays",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName2",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);
    }
}
