using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnOpeningStockQuantityNBIntoTableGoods : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "OpeningStockQuantityNB",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "OpeningStockQuantityNB",
            table: "Goods");
    }
}
