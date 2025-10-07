using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddConditionProduceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsProduceProduct",
            table: "ManufactureOrders",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 7, "ProduceProduct", "Gửi kho", "MANUFACTURE_ORDER" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.DropColumn(
            name: "IsProduceProduct",
            table: "ManufactureOrders");
    }
}
