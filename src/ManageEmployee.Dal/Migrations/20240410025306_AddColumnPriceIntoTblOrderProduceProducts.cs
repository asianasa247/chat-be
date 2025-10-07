using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnPriceIntoTblOrderProduceProducts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "Coefficient",
            table: "ProcedureRequestOvertimes",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<int>(
            name: "SymbolId",
            table: "ProcedureRequestOvertimes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<decimal>(
            name: "DiscountPrice",
            table: "OrderProduceProductDetails",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "TaxVAT",
            table: "OrderProduceProductDetails",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "UnitPrice",
            table: "OrderProduceProductDetails",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Coefficient",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.DropColumn(
            name: "SymbolId",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.DropColumn(
            name: "DiscountPrice",
            table: "OrderProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "TaxVAT",
            table: "OrderProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "UnitPrice",
            table: "OrderProduceProductDetails");
    }
}
