using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddNewColumnIntoInOutHistoryTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "TimeFrameFrom",
            table: "InOutHistories",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "TimeFrameTo",
            table: "InOutHistories",
            type: "datetime2",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TimeFrameFrom",
            table: "InOutHistories");

        migrationBuilder.DropColumn(
            name: "TimeFrameTo",
            table: "InOutHistories");
    }
}
