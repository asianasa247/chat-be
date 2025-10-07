using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableGoodWarehousePosition : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Image5",
            table: "GoodWarehouses");

        migrationBuilder.DropColumn(
            name: "Position",
            table: "GoodWarehouses");

        migrationBuilder.CreateTable(
            name: "GoodWarehousesPositions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodWarehousesId = table.Column<int>(type: "int", nullable: false),
                WareHouseShelvesId = table.Column<int>(type: "int", nullable: false),
                WareHouseFloorId = table.Column<int>(type: "int", nullable: false),
                WareHousePositionId = table.Column<int>(type: "int", nullable: false),
                Quatity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Warehouse = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodWarehousesPositions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UpdateArisingAccountQueue",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UpdateArisingAccountQueue", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodWarehousesPositions");

        migrationBuilder.DropTable(
            name: "UpdateArisingAccountQueue");

        migrationBuilder.AddColumn<string>(
            name: "Image5",
            table: "GoodWarehouses",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Position",
            table: "GoodWarehouses",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
