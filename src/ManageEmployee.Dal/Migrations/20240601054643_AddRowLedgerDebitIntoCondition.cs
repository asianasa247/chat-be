using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddRowLedgerDebitIntoCondition : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 5, "LedgerDebit", "Thêm công nợ kế toán", "ORDER_PRODUCE_PRODUCT" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 5);
    }
}
