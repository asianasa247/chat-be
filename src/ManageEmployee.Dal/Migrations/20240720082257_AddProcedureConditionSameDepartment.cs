using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddProcedureConditionSameDepartment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 9, "SameDepartment", "Cùng phòng ban", "REQUEST_EQUIPMENT" });

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name", "ProcedureCodes" },
            values: new object[] { 11, "SameDepartment", "Cùng phòng ban", "ADVANCE_PAYMENT" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 11);
    }
}
