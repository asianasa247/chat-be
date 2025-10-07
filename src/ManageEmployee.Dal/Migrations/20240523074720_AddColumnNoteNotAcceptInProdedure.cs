using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnNoteNotAcceptInProdedure : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "WarehouseProduceProducts",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "RequestExportGoods",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "NotAcceptCount",
            table: "ProcedureLogs",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "PlanningProduceProducts",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "PaymentProposals",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "OrderProduceProducts",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "ManufactureOrders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "GatePasses",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NoteNotAccept",
            table: "AdvancePayments",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "WarehouseProduceProducts");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "RequestExportGoods");

        migrationBuilder.DropColumn(
            name: "NotAcceptCount",
            table: "ProcedureLogs");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "PlanningProduceProducts");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "OrderProduceProducts");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "ManufactureOrders");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "GatePasses");

        migrationBuilder.DropColumn(
            name: "NoteNotAccept",
            table: "AdvancePayments");
    }
}
