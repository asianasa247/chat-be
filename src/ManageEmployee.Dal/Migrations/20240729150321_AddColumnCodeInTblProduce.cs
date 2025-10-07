using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCodeInTblProduce : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "WarehouseProduceProducts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "RequestExportGoods",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "RequestEquipments",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "ProduceProducts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "PlanningProduceProducts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "PaymentProposals",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "ManufactureOrders",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "LedgerProduceImports",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "LedgerProduceExports",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "GatePasses",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "ExpenditurePlans",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "CarLocations",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Code",
            table: "AdvancePayments",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Code",
            table: "WarehouseProduceProducts");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "RequestExportGoods");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "RequestEquipments");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "ProduceProducts");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "PlanningProduceProducts");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "ManufactureOrders");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "LedgerProduceImports");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "LedgerProduceExports");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "GatePasses");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "ExpenditurePlans");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "CarLocations");

        migrationBuilder.DropColumn(
            name: "Code",
            table: "AdvancePayments");
    }
}
