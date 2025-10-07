using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddShelveMAnyPosition : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WareHouseFloorWithPositions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                WareHouseFloorId = table.Column<int>(type: "int", nullable: false),
                WareHousePositionId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WareHouseFloorWithPositions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "WareHouseShelvesWithFloors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                WareHouseShelvesId = table.Column<int>(type: "int", nullable: false),
                WareHouseFloorId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WareHouseShelvesWithFloors", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WareHouseFloorWithPositions");

        migrationBuilder.DropTable(
            name: "WareHouseShelvesWithFloors");
    }
}
