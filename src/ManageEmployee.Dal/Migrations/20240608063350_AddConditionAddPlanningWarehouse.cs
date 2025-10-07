using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddConditionAddPlanningWarehouse : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "OrderProduceProducts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 6, "PlanningWarehouse", "Gửi kho", "PLANNING_PRODUCE_PRODUCT" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DropColumn(
            name: "Code",
            table: "OrderProduceProducts");
    }
}
