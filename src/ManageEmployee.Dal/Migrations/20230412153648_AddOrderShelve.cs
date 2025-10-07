using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddOrderShelve : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "OrderHorizontal",
            table: "WareHouseShelves",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "OrderVertical",
            table: "WareHouseShelves",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "WareHouseWithShelves",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                WareHouseId = table.Column<int>(type: "int", nullable: false),
                WareHouseShelveId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WareHouseWithShelves", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WareHouseWithShelves");

        migrationBuilder.DropColumn(
            name: "OrderHorizontal",
            table: "WareHouseShelves");

        migrationBuilder.DropColumn(
            name: "OrderVertical",
            table: "WareHouseShelves");
    }
}
