using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class AddColumnAmenityTypeIdsIntoTableGoodRoom : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AmenityIds",
            table: "GoodRoomTypes",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AmenityTypeIds",
            table: "GoodRoomTypes",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AmenityIds",
            table: "GoodRoomTypes");

        migrationBuilder.DropColumn(
            name: "AmenityTypeIds",
            table: "GoodRoomTypes");
    }
}
