using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial75 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "DiscountPrice",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "Price",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "SalePrice",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "TaxVAT",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateExpiration",
            table: "BillDetails",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateManufacture",
            table: "BillDetails",
            type: "datetime2",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DiscountPrice",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "Price",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "SalePrice",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "TaxVAT",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "DateExpiration",
            table: "BillDetails");

        migrationBuilder.DropColumn(
            name: "DateManufacture",
            table: "BillDetails");
    }
}
