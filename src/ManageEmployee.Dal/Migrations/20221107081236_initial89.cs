using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial89 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WebPriceEnglish",
            table: "Goods");

        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "Customers",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Order",
            table: "Customers");

        migrationBuilder.AddColumn<decimal>(
            name: "WebPriceEnglish",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true);
    }
}
