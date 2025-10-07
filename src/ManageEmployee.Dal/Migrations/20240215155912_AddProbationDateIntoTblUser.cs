using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddProbationDateIntoTblUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "ProbationFromAt",
            table: "Users",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "ProbationToAt",
            table: "Users",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "SalaryPercentage",
            table: "Users",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsInit",
            table: "P_ProcedureStatusSteps",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "P_ProcedureId",
            table: "P_ProcedureStatusRole",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ProbationFromAt",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "ProbationToAt",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "SalaryPercentage",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "IsInit",
            table: "P_ProcedureStatusSteps");

        migrationBuilder.DropColumn(
            name: "P_ProcedureId",
            table: "P_ProcedureStatusRole");
    }
}
