using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial84 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ProcedureNumber",
            table: "P_SalaryAdvance",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ProcedureNumber",
            table: "P_Leave",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ProcedureNumber",
            table: "P_Kpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ProcedureNumber",
            table: "P_SalaryAdvance");

        migrationBuilder.DropColumn(
            name: "ProcedureNumber",
            table: "P_Leave");

        migrationBuilder.DropColumn(
            name: "ProcedureNumber",
            table: "P_Kpis");
    }
}
