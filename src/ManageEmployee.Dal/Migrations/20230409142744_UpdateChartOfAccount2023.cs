using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateChartOfAccount2023 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"update ChartOfAccounts set Year = " + DateTime.Now.Year);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
