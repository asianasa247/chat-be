using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class SeedDataProcedureCondition : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "BillId",
            table: "OrderProduceProducts",
            type: "int",
            nullable: true);

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name" },
            values: new object[] { 2, "SendToWarehouse", "Gửi xuống kho" });

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name" },
            values: new object[] { 3, "SendToCashier", "Gửi cho bộ phận bán hàng" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DropColumn(
            name: "BillId",
            table: "OrderProduceProducts");
    }
}
