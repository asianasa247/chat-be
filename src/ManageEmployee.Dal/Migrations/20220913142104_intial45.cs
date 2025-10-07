using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial45 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CustomerQuote",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerQuote", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CustomerQuote_Detail",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                IdCustomerQuote = table.Column<long>(type: "bigint", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxVAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                StockUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerQuote_Detail", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CustomerQuote");

        migrationBuilder.DropTable(
            name: "CustomerQuote_Detail");
    }
}
