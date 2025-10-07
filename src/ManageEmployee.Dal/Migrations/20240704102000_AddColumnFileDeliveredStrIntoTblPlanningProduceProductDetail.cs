using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnFileDeliveredStrIntoTblPlanningProduceProductDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FileDeliveredStr",
            table: "PlanningProduceProductDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDone",
            table: "OrderProduceProducts",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDone",
            table: "OrderProduceProductDetails",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<double>(
            name: "QuantityDelivered",
            table: "OrderProduceProductDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<double>(
            name: "QuantityInProgress",
            table: "OrderProduceProductDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileDeliveredStr",
            table: "PlanningProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "IsDone",
            table: "OrderProduceProducts");

        migrationBuilder.DropColumn(
            name: "IsDone",
            table: "OrderProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "QuantityDelivered",
            table: "OrderProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "QuantityInProgress",
            table: "OrderProduceProductDetails");
    }
}
