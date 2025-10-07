using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnIsDoneInPlanningProcuceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDone",
            table: "PlanningProduceProducts",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<double>(
            name: "Qty",
            table: "GoodsPromotionDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<double>(
            name: "Qty",
            table: "BillPromotions",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDone",
            table: "PlanningProduceProducts");

        migrationBuilder.DropColumn(
            name: "Qty",
            table: "GoodsPromotionDetails");

        migrationBuilder.DropColumn(
            name: "Qty",
            table: "BillPromotions");
    }
}
