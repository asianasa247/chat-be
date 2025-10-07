using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnIsSpecialInTblGatePass : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsSpecial",
            table: "GatePasses",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.UpdateData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 4,
            column: "Code",
            value: "Special");

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 8, "Special", "Đặc biệt", "GATE_PASS" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 8);

        migrationBuilder.DropColumn(
            name: "IsSpecial",
            table: "GatePasses");

        migrationBuilder.UpdateData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 4,
            column: "Code",
            value: "SpecialOrder");
    }
}
