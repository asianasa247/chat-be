using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnAdvancePaymentIdIntoTblPaymentProposal : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AdvancePaymentCode",
            table: "PaymentProposals",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "AdvancePaymentId",
            table: "PaymentProposals",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RequestEquipmentCode",
            table: "PaymentProposals",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "RequestEquipmentId",
            table: "PaymentProposals",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RequestEquipmentOrderCode",
            table: "PaymentProposals",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "RequestEquipmentOrderId",
            table: "PaymentProposals",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AdvancePaymentCode",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "AdvancePaymentId",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "RequestEquipmentCode",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "RequestEquipmentId",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "RequestEquipmentOrderCode",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "RequestEquipmentOrderId",
            table: "PaymentProposals");
    }
}
