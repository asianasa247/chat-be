using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnPaymentProposalCodeInTblExpenditurePlanDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RequestEquipmentDetailId",
            table: "ExpenditurePlanDetails");

        migrationBuilder.RenameColumn(
            name: "RequestEquipmentCode",
            table: "ExpenditurePlanDetails",
            newName: "PaymentProposalCode");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PaymentProposalCode",
            table: "ExpenditurePlanDetails",
            newName: "RequestEquipmentCode");

        migrationBuilder.AddColumn<int>(
            name: "RequestEquipmentDetailId",
            table: "ExpenditurePlanDetails",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
