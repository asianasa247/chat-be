using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdarteColumnFromProcedureNoteInTblExpenditurePlanDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PaymentProposalNote",
            table: "ExpenditurePlanDetails",
            newName: "FromProcedureNote");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "FromProcedureNote",
            table: "ExpenditurePlanDetails",
            newName: "PaymentProposalNote");
    }
}
