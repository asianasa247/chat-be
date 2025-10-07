using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblBillPromotions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "PromotionAmount",
            table: "Bills",
            type: "float",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "BillPromotions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BillId = table.Column<int>(type: "int", nullable: false),
                GoodsPromotionDetailId = table.Column<int>(type: "int", nullable: false),
                GoodsPromotionId = table.Column<int>(type: "int", nullable: false),
                Standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Discount = table.Column<double>(type: "float", nullable: false),
                Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Detail1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Detail1Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Detail2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Detail2Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BillPromotions", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "BillPromotions");

        migrationBuilder.DropColumn(
            name: "PromotionAmount",
            table: "Bills");
    }
}
