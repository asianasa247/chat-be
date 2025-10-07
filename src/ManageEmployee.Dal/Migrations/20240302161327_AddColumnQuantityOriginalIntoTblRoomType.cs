using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class AddColumnQuantityOriginalIntoTblRoomType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "BillId",
            table: "Order",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "QuantityOriginal",
            table: "GoodRoomTypes",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BillId",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "QuantityOriginal",
            table: "GoodRoomTypes");
    }
}
