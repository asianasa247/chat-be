using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblGoodsQuota : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "GoodsQuotaId",
            table: "GoodDetails",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "GoodsQuotaDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodsQuotaId = table.Column<int>(type: "int", nullable: false),
                Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Detail1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DetailName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Detail2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DetailName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Warehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsQuotaDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GoodsQuotaRecipes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsQuotaRecipes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GoodsQuotas",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodsQuotaRecipeId = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsQuotas", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodsQuotaDetails");

        migrationBuilder.DropTable(
            name: "GoodsQuotaRecipes");

        migrationBuilder.DropTable(
            name: "GoodsQuotas");

        migrationBuilder.DropColumn(
            name: "GoodsQuotaId",
            table: "GoodDetails");
    }
}
