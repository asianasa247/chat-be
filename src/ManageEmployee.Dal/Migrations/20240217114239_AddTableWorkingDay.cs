using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableWorkingDay : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Date",
            table: "ProcedureRequestOvertimes",
            newName: "ToAt");

        migrationBuilder.RenameColumn(
            name: "isFinish",
            table: "P_SalaryAdvance",
            newName: "IsFinish");

        migrationBuilder.RenameColumn(
            name: "isFinish",
            table: "P_Leave",
            newName: "IsFinish");

        migrationBuilder.AddColumn<DateTime>(
            name: "FromAt",
            table: "ProcedureRequestOvertimes",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.CreateTable(
            name: "WorkingDays",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Days = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Holidays = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Year = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkingDays", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WorkingDays");

        migrationBuilder.DropColumn(
            name: "FromAt",
            table: "ProcedureRequestOvertimes");

        migrationBuilder.RenameColumn(
            name: "ToAt",
            table: "ProcedureRequestOvertimes",
            newName: "Date");

        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "P_SalaryAdvance",
            newName: "isFinish");

        migrationBuilder.RenameColumn(
            name: "IsFinish",
            table: "P_Leave",
            newName: "isFinish");
    }
}
