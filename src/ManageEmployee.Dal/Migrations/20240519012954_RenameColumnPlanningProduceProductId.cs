using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RenameColumnPlanningProduceProductId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ProduceProductId",
            table: "PlanningProduceProductDetails",
            newName: "PlanningProduceProductId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PlanningProduceProductId",
            table: "PlanningProduceProductDetails",
            newName: "ProduceProductId");
    }
}
