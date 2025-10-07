using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTablePositionWareHouse : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WareHouseFloors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WareHouseFloors", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "WareHousePositions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WareHousePositions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "WareHouseShelves",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WareHouseShelves", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WareHouseFloors");

        migrationBuilder.DropTable(
            name: "WareHousePositions");

        migrationBuilder.DropTable(
            name: "WareHouseShelves");
    }
}
