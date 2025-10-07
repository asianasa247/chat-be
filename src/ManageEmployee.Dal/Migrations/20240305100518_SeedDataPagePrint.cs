using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class SeedDataPagePrint : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Avatar",
            table: "Users",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.InsertData(
            table: "PagePrints",
            columns: new[] { "Id", "Height", "Width" },
            values: new object[] { 1, 82, 20 });

        migrationBuilder.InsertData(
            table: "Prints",
            columns: new[] { "Id", "Height", "MarginBottom", "MarginLeft", "MarginRight", "MarginTop", "Name", "Size", "Width" },
            values: new object[] { 1, null, 10.0, 10.0, 10.0, 10.0, "QrCode", 16, null });

        migrationBuilder.InsertData(
            table: "Prints",
            columns: new[] { "Id", "Height", "MarginBottom", "MarginLeft", "MarginRight", "MarginTop", "Name", "Size", "Width" },
            values: new object[] { 2, 10, 10.0, 10.0, 10.0, 10.0, "Barcode", null, 10 });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "PagePrints",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Prints",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Prints",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.AlterColumn<string>(
            name: "Avatar",
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
