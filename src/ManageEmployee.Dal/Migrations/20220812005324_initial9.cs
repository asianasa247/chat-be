using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial9 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BillDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BillId = table.Column<int>(type: "int", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                Quality = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxVAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BillDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Bills",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DeskId = table.Column<int>(type: "int", nullable: false),
                FloorId = table.Column<int>(type: "int", nullable: false),
                UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                QuantityCustomer = table.Column<int>(type: "int", nullable: false),
                TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AmountReceivedByCus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AmountSendToCus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                TypePay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DisplayOrder = table.Column<long>(type: "bigint", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bills", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BillTrackings",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BillId = table.Column<int>(type: "int", nullable: false),
                UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TranType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsRead = table.Column<bool>(type: "bit", nullable: false),
                IsImportant = table.Column<bool>(type: "bit", nullable: false),
                UserCodeReceived = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                DisplayOrder = table.Column<long>(type: "bigint", nullable: false),
                Prioritize = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BillTrackings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Goods",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MenuType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PriceList = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MenuWeb = table.Column<string>(type: "nvarchar(max)", nullable: false),
                GoodsType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Inventory = table.Column<long>(type: "bigint", nullable: false),
                Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Delivery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MinStockLevel = table.Column<long>(type: "bigint", nullable: false),
                MaxStockLevel = table.Column<long>(type: "bigint", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Warehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Image1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Image2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Image3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Image4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Image5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                WebGoodNameVietNam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WebPriceVietNam = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                WebDiscountVietNam = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                TitleVietNam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ContentVietNam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WebGoodNameKorea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WebPriceKorea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                WebDiscountKorea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                TitleKorea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ContentKorea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WebGoodNameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WebPriceEnglish = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                WebDiscountEnglish = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                TitleEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ContentEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TaxVAT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                isPromotion = table.Column<bool>(type: "bit", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Goods", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "BillDetails");

        migrationBuilder.DropTable(
            name: "Bills");

        migrationBuilder.DropTable(
            name: "BillTrackings");

        migrationBuilder.DropTable(
            name: "Goods");
    }
}
