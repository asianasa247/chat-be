using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial125 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GoodId",
            table: "CategoryStatusWebPeriods");

        migrationBuilder.CreateTable(
            name: "CategoryStatusWebPeriodGoods",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CategoryStatusWebPeriodId = table.Column<int>(type: "int", nullable: false),
                GoodId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoryStatusWebPeriodGoods", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CategoryStatusWebPeriodGoods");

        migrationBuilder.AddColumn<string>(
            name: "GoodId",
            table: "CategoryStatusWebPeriods",
            type: "nvarchar(max)",
            nullable: true);
    }
}
