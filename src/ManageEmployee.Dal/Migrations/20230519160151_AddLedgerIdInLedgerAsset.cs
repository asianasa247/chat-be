using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddLedgerIdInLedgerAsset : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LedgerFixedAssets",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LedgerId = table.Column<long>(type: "bigint", nullable: false),
                FixedAssetId = table.Column<int>(type: "int", nullable: false),
                FixedAsset242Id = table.Column<int>(type: "int", nullable: false),
                IsInternal = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerFixedAssets", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerFixedAssets");
    }
}
