using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCarIdInTblBillPromotions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CarId",
            table: "BillPromotions",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CarName",
            table: "BillPromotions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "CustomerId",
            table: "BillPromotions",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CarId",
            table: "BillPromotions");

        migrationBuilder.DropColumn(
            name: "CarName",
            table: "BillPromotions");

        migrationBuilder.DropColumn(
            name: "CustomerId",
            table: "BillPromotions");
    }
}
