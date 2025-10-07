using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class CreateSalaryTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Salaries",
            columns: table => new
            {
                Userid = table.Column<int>(type: "int", nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                Year = table.Column<int>(type: "int", nullable: false),
                NumberOfWorkingDays = table.Column<double>(type: "float", nullable: false),
                BaseSalary = table.Column<double>(type: "float", nullable: false),
                ContractualSalaryAmount = table.Column<double>(type: "float", nullable: false),
                AllowanceAmount = table.Column<double>(type: "float", nullable: false),
                DeduceMealCost = table.Column<double>(type: "float", nullable: false),
                RemainingAmount = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Salaries", x => new { x.Userid, x.Month, x.Year });
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Salaries");
    }
}
