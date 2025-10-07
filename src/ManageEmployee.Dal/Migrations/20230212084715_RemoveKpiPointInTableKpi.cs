using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RemoveKpiPointInTableKpi : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Percent",
            table: "P_Kpi_Items");

        migrationBuilder.DropColumn(
            name: "PointKpi",
            table: "P_Kpi_Items");

        migrationBuilder.RenameColumn(
            name: "isFinish",
            table: "P_Kpis",
            newName: "IsFinish");

        migrationBuilder.AddColumn<int>(
            name: "Year",
            table: "Ledgers",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Year",
            table: "LedgerInternals",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Year",
            table: "Ledgers");

        migrationBuilder.DropColumn(
            name: "Year",
            table: "LedgerInternals");

        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "P_Kpis",
            newName: "isFinish");

        migrationBuilder.AddColumn<double>(
            name: "Percent",
            table: "P_Kpi_Items",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "PointKpi",
            table: "P_Kpi_Items",
            type: "float",
            nullable: true);
    }
}
