using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnYearInTableLedger : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_CategoryStatusWebPeriods_CategoryId",
            table: "CategoryStatusWebPeriods",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_CategoryStatusWebPeriodGoods_CategoryStatusWebPeriodId",
            table: "CategoryStatusWebPeriodGoods",
            column: "CategoryStatusWebPeriodId");

        migrationBuilder.CreateIndex(
            name: "IX_CategoryStatusWebPeriodGoods_GoodId",
            table: "CategoryStatusWebPeriodGoods",
            column: "GoodId");

        migrationBuilder.AddForeignKey(
            name: "FK_CategoryStatusWebPeriodGoods_CategoryStatusWebPeriods_CategoryStatusWebPeriodId",
            table: "CategoryStatusWebPeriodGoods",
            column: "CategoryStatusWebPeriodId",
            principalTable: "CategoryStatusWebPeriods",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        //migrationBuilder.AddForeignKey(
        //    name: "FK_CategoryStatusWebPeriodGoods_Goods_GoodId",
        //    table: "CategoryStatusWebPeriodGoods",
        //    column: "GoodId",
        //    principalTable: "Goods",
        //    principalColumn: "Id",
        //    onDelete: ReferentialAction.Cascade);

        //migrationBuilder.AddForeignKey(
        //    name: "FK_CategoryStatusWebPeriods_Categories_CategoryId",
        //    table: "CategoryStatusWebPeriods",
        //    column: "CategoryId",
        //    principalTable: "Categories",
        //    principalColumn: "Id",
        //    onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CategoryStatusWebPeriodGoods_CategoryStatusWebPeriods_CategoryStatusWebPeriodId",
            table: "CategoryStatusWebPeriodGoods");

        migrationBuilder.DropForeignKey(
            name: "FK_CategoryStatusWebPeriodGoods_Goods_GoodId",
            table: "CategoryStatusWebPeriodGoods");

        migrationBuilder.DropForeignKey(
            name: "FK_CategoryStatusWebPeriods_Categories_CategoryId",
            table: "CategoryStatusWebPeriods");

        migrationBuilder.DropIndex(
            name: "IX_CategoryStatusWebPeriods_CategoryId",
            table: "CategoryStatusWebPeriods");

        migrationBuilder.DropIndex(
            name: "IX_CategoryStatusWebPeriodGoods_CategoryStatusWebPeriodId",
            table: "CategoryStatusWebPeriodGoods");

        migrationBuilder.DropIndex(
            name: "IX_CategoryStatusWebPeriodGoods_GoodId",
            table: "CategoryStatusWebPeriodGoods");
    }
}
