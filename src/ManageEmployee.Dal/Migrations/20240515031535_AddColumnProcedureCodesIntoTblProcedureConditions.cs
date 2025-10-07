using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnProcedureCodesIntoTblProcedureConditions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ProcedureCodes",
            table: "ProcedureConditions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 1,
            column: "ProcedureCodes",
            value: "ORDER_PRODUCE_PRODUCT");

        migrationBuilder.UpdateData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 2,
            column: "ProcedureCodes",
            value: "ORDER_PRODUCE_PRODUCT");

        migrationBuilder.UpdateData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 3,
            column: "ProcedureCodes",
            value: "ORDER_PRODUCE_PRODUCT");

        migrationBuilder.UpdateData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 4,
            column: "ProcedureCodes",
            value: "ORDER_PRODUCE_PRODUCT");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ProcedureCodes",
            table: "ProcedureConditions");
    }
}
