using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableChucVu25845 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoodsWebsId",
                table: "CategoryStatusWebPeriodGoods",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GoodsWebs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuType = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    PriceList = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    GoodsType = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    SalePrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    DiscountPrice = table.Column<double>(type: "float", nullable: false),
                    Inventory = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Delivery = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MinStockLevel = table.Column<long>(type: "bigint", nullable: false),
                    MaxStockLevel = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Warehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    WarehouseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DetailName1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DetailName2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail1English = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailName1English = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detail1Korean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailName1Korean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Image2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Image3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Image4 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Image5 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    WebGoodNameVietNam = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebPriceVietNam = table.Column<double>(type: "float", nullable: true),
                    WebDiscountVietNam = table.Column<double>(type: "float", nullable: true),
                    TitleVietNam = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContentVietNam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebGoodNameKorea = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebPriceKorea = table.Column<double>(type: "float", nullable: true),
                    WebDiscountKorea = table.Column<double>(type: "float", nullable: true),
                    TitleKorea = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ContentKorea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebGoodNameEnglish = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WebPriceEnglish = table.Column<double>(type: "float", nullable: true),
                    WebDiscountEnglish = table.Column<double>(type: "float", nullable: true),
                    TitleEnglish = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ContentEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isPromotion = table.Column<bool>(type: "bit", nullable: true),
                    DateManufacture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StockUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateApplicable = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Net = table.Column<double>(type: "float", nullable: true),
                    TaxRateId = table.Column<long>(type: "bigint", nullable: true),
                    OpeningStockQuantityNB = table.Column<double>(type: "float", nullable: true),
                    IsService = table.Column<bool>(type: "bit", nullable: false),
                    GoodsQuotaId = table.Column<int>(type: "int", nullable: true),
                    NumberItem = table.Column<int>(type: "int", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsWebs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryStatusWebPeriodGoods_GoodsWebsId",
                table: "CategoryStatusWebPeriodGoods",
                column: "GoodsWebsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryStatusWebPeriodGoods_GoodsWebs_GoodsWebsId",
                table: "CategoryStatusWebPeriodGoods",
                column: "GoodsWebsId",
                principalTable: "GoodsWebs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryStatusWebPeriodGoods_GoodsWebs_GoodsWebsId",
                table: "CategoryStatusWebPeriodGoods");

            migrationBuilder.DropTable(
                name: "GoodsWebs");

            migrationBuilder.DropIndex(
                name: "IX_CategoryStatusWebPeriodGoods_GoodsWebsId",
                table: "CategoryStatusWebPeriodGoods");

            migrationBuilder.DropColumn(
                name: "GoodsWebsId",
                table: "CategoryStatusWebPeriodGoods");
        }
    }
}
