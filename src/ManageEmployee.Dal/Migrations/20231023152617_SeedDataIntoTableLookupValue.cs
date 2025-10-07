using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class SeedDataIntoTableLookupValue : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("delete from LookupValues");
        migrationBuilder.InsertData(
            table: "LookupValues",
            columns: new[] { "Id", "Code", "Description", "Scope", "Value" },
            values: new object[,]
            {
                { 1, "1", null, "COA_ACC_GROUP", "1. Thông thường" },
                { 2, "2", null, "COA_ACC_GROUP", "2. Khách hàng" },
                { 3, "3", null, "COA_ACC_GROUP", "3. Tồn kho" },
                { 4, "4", null, "COA_ACC_GROUP", "4. Nhập xuất" },
                { 5, "1", null, "COA_CLASSIFICATION", "1. Thông thường" },
                { 6, "2", null, "COA_CLASSIFICATION", "2. Hàng hóa" },
                { 7, "3", null, "COA_CLASSIFICATION", "3. Nguyên vật liệu" },
                { 8, "4", null, "COA_CLASSIFICATION", "4. Công cụ dụng cụ" },
                { 9, "5", null, "COA_CLASSIFICATION", "5. Tài sản cố định" },
                { 10, "6", null, "COA_CLASSIFICATION", "6. Dự án" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "LookupValues",
            keyColumn: "Id",
            keyValue: 10);
    }
}
