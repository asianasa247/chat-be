using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateYearInAccount : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        int year = DateTime.Now.Year;
        migrationBuilder.Sql($"update ChartOfAccountGroupLinks set  [Year]  ={year} where  [Year]  = 0; update ChartOfAccountGroups set  [Year]  = {year} where  [Year]  = 0;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
