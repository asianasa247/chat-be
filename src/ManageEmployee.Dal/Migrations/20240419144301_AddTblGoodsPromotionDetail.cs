using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblGoodsPromotionDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "UserName",
            table: "SalaryUserVersions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "GoodsPromotionDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodsPromotionId = table.Column<int>(type: "int", nullable: false),
                QuantityFrom = table.Column<double>(type: "float", nullable: false),
                QuantityAt = table.Column<double>(type: "float", nullable: false),
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
                table.PrimaryKey("PK_GoodsPromotionDetails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodsPromotionDetails");

        migrationBuilder.DropColumn(
            name: "UserName",
            table: "SalaryUserVersions");
    }
}
