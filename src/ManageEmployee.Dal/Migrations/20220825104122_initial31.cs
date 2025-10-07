using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial31 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ClosingCredit",
            table: "ChartOfAccounts");

        migrationBuilder.DropColumn(
            name: "ClosingDebit",
            table: "ChartOfAccounts");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "ClosingCredit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "ClosingDebit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true);
    }
}
