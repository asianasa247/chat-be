using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddConditionSendToCashierForPlanningProduce : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "GatePasses",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 12, "SendToCashier", "Gửi cho bộ phận bán hàng", "PLANNING_PRODUCE_PRODUCT" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 12);

        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "GatePasses");
    }
}
