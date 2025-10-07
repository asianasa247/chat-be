using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnNoteInTblBillPromotions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "GoodsPromotionCode",
            table: "BillPromotions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "GoodsPromotionName",
            table: "BillPromotions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Note",
            table: "BillPromotions",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GoodsPromotionCode",
            table: "BillPromotions");

        migrationBuilder.DropColumn(
            name: "GoodsPromotionName",
            table: "BillPromotions");

        migrationBuilder.DropColumn(
            name: "Note",
            table: "BillPromotions");
    }
}
