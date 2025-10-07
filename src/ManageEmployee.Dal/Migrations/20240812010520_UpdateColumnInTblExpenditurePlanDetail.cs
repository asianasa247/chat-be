using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInTblExpenditurePlanDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PaymentProposalId",
            table: "ExpenditurePlanDetails",
            newName: "FromProcedureId");

        migrationBuilder.RenameColumn(
            name: "PaymentProposalCode",
            table: "ExpenditurePlanDetails",
            newName: "FromTableName");

        migrationBuilder.AddColumn<string>(
            name: "FromProcedureCode",
            table: "ExpenditurePlanDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FromProcedureCode",
            table: "ExpenditurePlanDetails");

        migrationBuilder.RenameColumn(
            name: "FromTableName",
            table: "ExpenditurePlanDetails",
            newName: "PaymentProposalCode");

        migrationBuilder.RenameColumn(
            name: "FromProcedureId",
            table: "ExpenditurePlanDetails",
            newName: "PaymentProposalId");
    }
}
