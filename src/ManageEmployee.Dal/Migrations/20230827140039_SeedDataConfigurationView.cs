using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class SeedDataConfigurationView : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "ConfigurationViews",
            columns: new[] { "Id", "FieldName", "Value", "ViewName" },
            values: new object[,]
            {
                { 1, "TypePay", "[\r\n                        { label: 'Tiền mặt', value: 'TM' },\r\n                                { label: 'Công nợ', value: 'CN' },\r\n                                { label: 'Ngân hàng', value: 'NH' },\r\n                    ]", "cashier" },
                { 2, "Layout", "list", "cashier" },
                { 3, "PrintBill", "ExporttBill,DeliveryBill,BillPrint", "cashier" },
                { 4, "QuantityBoxNec", "1", "cashier" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ConfigurationViews",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "ConfigurationViews",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "ConfigurationViews",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "ConfigurationViews",
            keyColumn: "Id",
            keyValue: 4);
    }
}
