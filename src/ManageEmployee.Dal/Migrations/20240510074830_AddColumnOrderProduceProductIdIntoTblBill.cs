using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnOrderProduceProductIdIntoTblBill : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BillId",
            table: "OrderProduceProducts");

        migrationBuilder.AddColumn<int>(
            name: "OrderProduceProductId",
            table: "Bills",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "OrderProduceProductId",
            table: "Bills");

        migrationBuilder.AddColumn<int>(
            name: "BillId",
            table: "OrderProduceProducts",
            type: "int",
            nullable: true);
    }
}
