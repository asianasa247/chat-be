using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnLedgerInternalIdInTableLedgerFixedAsset : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "LedgerInternalId",
            table: "LedgerFixedAssets",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LedgerInternalId",
            table: "LedgerFixedAssets");
    }
}
