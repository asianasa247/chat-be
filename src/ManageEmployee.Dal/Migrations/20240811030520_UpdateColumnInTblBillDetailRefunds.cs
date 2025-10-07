using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInTblBillDetailRefunds : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "BillPromotions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "BillDetailRefunds",
            type: "float",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "GoodsId",
            table: "BillDetailRefunds",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "BillDetailId",
            table: "BillDetailRefunds",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddColumn<int>(
            name: "BillPromotionId",
            table: "BillDetailRefunds",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "UnitPrice",
            table: "BillDetailRefunds",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Status",
            table: "BillPromotions");

        migrationBuilder.DropColumn(
            name: "BillPromotionId",
            table: "BillDetailRefunds");

        migrationBuilder.DropColumn(
            name: "UnitPrice",
            table: "BillDetailRefunds");

        migrationBuilder.AlterColumn<int>(
            name: "Quantity",
            table: "BillDetailRefunds",
            type: "int",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<int>(
            name: "GoodsId",
            table: "BillDetailRefunds",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "BillDetailId",
            table: "BillDetailRefunds",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }
}
