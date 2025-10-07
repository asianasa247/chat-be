using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableGoodsPriceList : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GoodsPriceList",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                PriceList = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsPriceList", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodsPriceList");
    }
}
