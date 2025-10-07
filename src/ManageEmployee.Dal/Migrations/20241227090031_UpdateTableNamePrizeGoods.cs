using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class UpdateTableNamePrizeGoods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodPromotions");

            migrationBuilder.CreateTable(
                name: "PrizeGoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSettingsSpin = table.Column<int>(type: "int", nullable: false),
                    PrizeId = table.Column<int>(type: "int", nullable: false),
                    GoodId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrizeGoods", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrizeGoods");

            migrationBuilder.CreateTable(
                name: "GoodPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodId = table.Column<int>(type: "int", nullable: false),
                    IdSettingsSpin = table.Column<int>(type: "int", nullable: false),
                    PrizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodPromotions", x => x.Id);
                });
        }
    }
}
