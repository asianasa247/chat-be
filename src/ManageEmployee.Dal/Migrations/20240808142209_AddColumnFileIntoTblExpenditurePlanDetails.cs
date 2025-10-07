using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnFileIntoTblExpenditurePlanDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "ExpenditurePlans");

        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "ExpenditurePlanDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PaymentProposalNote",
            table: "ExpenditurePlanDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "ExpenditurePlanDetails");

        migrationBuilder.DropColumn(
            name: "PaymentProposalNote",
            table: "ExpenditurePlanDetails");

        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "ExpenditurePlans",
            type: "nvarchar(max)",
            nullable: true);
    }
}
