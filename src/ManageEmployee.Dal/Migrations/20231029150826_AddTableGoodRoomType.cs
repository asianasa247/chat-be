using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class AddTableGoodRoomType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GoodRoomPrices",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodId = table.Column<int>(type: "int", nullable: false),
                PriceShow = table.Column<double>(type: "float", nullable: false),
                Discount = table.Column<double>(type: "float", nullable: false),
                Price = table.Column<double>(type: "float", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAt = table.Column<int>(type: "int", nullable: false),
                CreatedBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<int>(type: "int", nullable: false),
                UpdatedBy = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodRoomPrices", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GoodRoomTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                RoomType = table.Column<int>(type: "int", nullable: false),
                LengthRoom = table.Column<double>(type: "float", nullable: true),
                WidthRoom = table.Column<double>(type: "float", nullable: true),
                AdultQuantity = table.Column<int>(type: "int", nullable: true),
                ChildrenQuantity = table.Column<int>(type: "int", nullable: true),
                IsExtraBed = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodRoomTypes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodRoomPrices");

        migrationBuilder.DropTable(
            name: "GoodRoomTypes");
    }
}
