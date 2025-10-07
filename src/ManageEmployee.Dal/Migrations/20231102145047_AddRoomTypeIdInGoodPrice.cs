using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class AddRoomTypeIdInGoodPrice : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "GoodRoomPrices",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "CreatedBy",
            table: "GoodRoomPrices",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "GoodRoomPrices",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "UpdatedBy",
            table: "GoodRoomPrices",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "GoodRoomPrices");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "GoodRoomPrices");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "GoodRoomPrices");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "GoodRoomPrices");
    }
}
