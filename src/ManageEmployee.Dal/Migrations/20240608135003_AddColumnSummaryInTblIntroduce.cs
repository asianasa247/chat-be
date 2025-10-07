using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnSummaryInTblIntroduce : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CarName",
            table: "WarehouseProduceProductDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Summary",
            table: "Introduces",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CarName",
            table: "WarehouseProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "Summary",
            table: "Introduces");
    }
}
