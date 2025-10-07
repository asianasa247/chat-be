using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial123 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "FixedAsset242Id",
            table: "LedgerInternals",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "CategoryStatusWebPeriods",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CategoryId = table.Column<int>(type: "int", nullable: false),
                FromAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ToAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                GoodId = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoryStatusWebPeriods", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CategoryStatusWebPeriods");

        migrationBuilder.DropColumn(
            name: "FixedAsset242Id",
            table: "LedgerInternals");
    }
}
