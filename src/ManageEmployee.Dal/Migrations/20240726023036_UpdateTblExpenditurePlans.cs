using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateTblExpenditurePlans : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GoodType",
            table: "ExpenditurePlanDetails");

        migrationBuilder.RenameColumn(
            name: "RequestEquipmentId",
            table: "ExpenditurePlanDetails",
            newName: "PaymentProposalId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PaymentProposalId",
            table: "ExpenditurePlanDetails",
            newName: "RequestEquipmentId");

        migrationBuilder.AddColumn<string>(
            name: "GoodType",
            table: "ExpenditurePlanDetails",
            type: "nvarchar(max)",
            nullable: true);
    }
}
