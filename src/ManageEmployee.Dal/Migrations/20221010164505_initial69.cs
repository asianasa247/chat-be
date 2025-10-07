using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial69 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GoodWarehouses",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MenuType = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                PriceList = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                GoodsType = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Inventory = table.Column<long>(type: "bigint", nullable: false),
                Position = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Delivery = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                Account = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                AccountName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Warehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                WarehouseName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Detail1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DetailName1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Detail2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DetailName2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Image1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Image2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Image3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Image4 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Image5 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                DateManufacture = table.Column<DateTime>(type: "datetime2", nullable: true),
                DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Order = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodWarehouses", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodWarehouses");
    }
}
