using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class ReplaceUserIdToUserIdStrInTblProcedureRequestOvertime : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserId",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.AddColumn<string>(
            name: "UserIdStr",
            table: "ProcedureRequestOvertimes",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserIdStr",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "ProcedureRequestOvertimes",
            type: "int",
            nullable: true);
    }
}
