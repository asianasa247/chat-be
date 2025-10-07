using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial94 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Delivery",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "DiscountPrice",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "Image2",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "Image3",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "Image4",
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

        migrationBuilder.AddColumn<string>(
            name: "Note",
            table: "GoodWarehouses",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Note",
            table: "GoodWarehouses");

        migrationBuilder.AddColumn<string>(
            name: "Delivery",
            table: "GoodWarehouses",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "DiscountPrice",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<string>(
            name: "Image2",
            table: "GoodWarehouses",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Image3",
            table: "GoodWarehouses",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Image4",
            table: "GoodWarehouses",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

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
    }
}
