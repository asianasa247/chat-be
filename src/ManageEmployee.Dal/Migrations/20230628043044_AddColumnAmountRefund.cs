using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnAmountRefund : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "AmountRefund",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "BillDetails",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AmountRefund",
            table: "Bills");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "BillDetails");
    }
}
