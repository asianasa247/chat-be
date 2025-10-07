using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnRefundAmountIntoTblPaymentProposals : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "TotalAount",
            table: "PaymentProposals",
            newName: "TotalAmount");

        migrationBuilder.RenameColumn(
            name: "Aount",
            table: "PaymentProposalDetails",
            newName: "Amount");

        migrationBuilder.AddColumn<double>(
            name: "AdvanceAmount",
            table: "PaymentProposals",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<bool>(
            name: "IsImmediate",
            table: "PaymentProposals",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<double>(
            name: "RefundAmount",
            table: "PaymentProposals",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<int>(
            name: "CarId",
            table: "GatePasses",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AdvanceAmount",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "IsImmediate",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "RefundAmount",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "CarId",
            table: "GatePasses");

        migrationBuilder.RenameColumn(
            name: "TotalAmount",
            table: "PaymentProposals",
            newName: "TotalAount");

        migrationBuilder.RenameColumn(
            name: "Amount",
            table: "PaymentProposalDetails",
            newName: "Aount");
    }
}
