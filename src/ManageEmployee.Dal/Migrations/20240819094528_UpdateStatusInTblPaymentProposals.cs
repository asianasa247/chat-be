using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateStatusInTblPaymentProposals : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDone",
            table: "PaymentProposals",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsInprogress",
            table: "PaymentProposals",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsPart",
            table: "PaymentProposals",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDone",
            table: "ExpenditurePlans",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsPart",
            table: "ExpenditurePlans",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDone",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "IsInprogress",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "IsPart",
            table: "PaymentProposals");

        migrationBuilder.DropColumn(
            name: "IsDone",
            table: "ExpenditurePlans");

        migrationBuilder.DropColumn(
            name: "IsPart",
            table: "ExpenditurePlans");
    }
}
