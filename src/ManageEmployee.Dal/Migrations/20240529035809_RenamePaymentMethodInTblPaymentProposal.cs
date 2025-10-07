using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RenamePaymentMethodInTblPaymentProposal : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PaymentMenthod",
            table: "PaymentProposals",
            newName: "PaymentMethod");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PaymentMethod",
            table: "PaymentProposals",
            newName: "PaymentMenthod");
    }
}
