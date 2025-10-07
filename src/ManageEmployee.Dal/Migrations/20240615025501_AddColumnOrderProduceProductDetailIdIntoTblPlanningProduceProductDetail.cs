using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnOrderProduceProductDetailIdIntoTblPlanningProduceProductDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "DiscountPrice",
            table: "PlanningProduceProductDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<int>(
            name: "OrderProduceProductDetailId",
            table: "PlanningProduceProductDetails",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "OrderProduceProductId",
            table: "PlanningProduceProductDetails",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<double>(
            name: "TaxVAT",
            table: "PlanningProduceProductDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<double>(
            name: "UnitPrice",
            table: "PlanningProduceProductDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DiscountPrice",
            table: "PlanningProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "OrderProduceProductDetailId",
            table: "PlanningProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "OrderProduceProductId",
            table: "PlanningProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "TaxVAT",
            table: "PlanningProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "UnitPrice",
            table: "PlanningProduceProductDetails");
    }
}
