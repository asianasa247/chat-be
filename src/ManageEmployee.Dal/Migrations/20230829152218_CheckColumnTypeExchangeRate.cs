using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class CheckColumnTypeExchangeRate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "ExchangeRate",
            table: "Ledgers",
            type: "decimal(18,4)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "ExchangeRate",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,4)");
    }
}
