using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCarNameIntoPlanningProduceProductDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CarName",
            table: "PlanningProduceProductDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TableId",
            table: "PaymentProposals",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TableName",
            table: "PaymentProposals",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CarName",
            table: "PlanningProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "TableId",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "TableName",
            table: "PaymentProposals");
    }
}
