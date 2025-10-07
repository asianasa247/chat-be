using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateDataSaleYear : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@" delete from [dbo].[YearSales]");

        migrationBuilder.Sql(@"   INSERT INTO [dbo].[YearSales]
                                           ([Year]
                                           ,[Note])
                                     VALUES
                                           (2022
                                           ,''),
		                                   (2023
                                           ,'')
                                GO
                             ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
