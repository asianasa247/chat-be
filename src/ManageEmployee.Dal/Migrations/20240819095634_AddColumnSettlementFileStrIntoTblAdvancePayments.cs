using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnSettlementFileStrIntoTblAdvancePayments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDone",
            table: "AdvancePayments",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "SettlementFileStr",
            table: "AdvancePayments",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDone",
            table: "AdvancePayments");

        migrationBuilder.DropColumn(
            name: "SettlementFileStr",
            table: "AdvancePayments");
    }
}
