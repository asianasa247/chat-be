using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class RemoveDateAtInGoodPrice : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "GoodRoomPrices");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "GoodRoomPrices");

        migrationBuilder.DropColumn(
            name: "GoodId",
            table: "GoodRoomPrices");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "GoodRoomPrices");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "GoodRoomPrices",
            newName: "RoomTypeId");

        migrationBuilder.AddColumn<DateTime>(
            name: "Date",
            table: "Bills",
            type: "datetime2",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Date",
            table: "Bills");

        migrationBuilder.RenameColumn(
            name: "RoomTypeId",
            table: "GoodRoomPrices",
            newName: "UpdatedAt");

        migrationBuilder.AddColumn<int>(
            name: "CreatedAt",
            table: "GoodRoomPrices",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedBy",
            table: "GoodRoomPrices",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "GoodId",
            table: "GoodRoomPrices",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedBy",
            table: "GoodRoomPrices",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }
}
