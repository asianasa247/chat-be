using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateDataYearForUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"update Users set YearCurrent = " + DateTime.Now.Year + " where YearCurrent = 0");

    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
