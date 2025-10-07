using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnPlanningProduceProductIntoTblPlanningProduceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreateFromTable",
            table: "PlanningProduceProducts",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "ShouldExport",
            table: "PlanningProduceProductDetails",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreateFromTable",
            table: "PlanningProduceProducts");

        migrationBuilder.DropColumn(
            name: "ShouldExport",
            table: "PlanningProduceProductDetails");
    }
}
