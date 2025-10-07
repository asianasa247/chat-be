using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnStandardIntoTblGoodsPromotionDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "QuantityAt",
            table: "GoodsPromotionDetails");

        migrationBuilder.DropColumn(
            name: "QuantityFrom",
            table: "GoodsPromotionDetails");

        migrationBuilder.AddColumn<string>(
            name: "Standard",
            table: "GoodsPromotionDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Standard",
            table: "GoodsPromotionDetails");

        migrationBuilder.AddColumn<double>(
            name: "QuantityAt",
            table: "GoodsPromotionDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<double>(
            name: "QuantityFrom",
            table: "GoodsPromotionDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }
}
