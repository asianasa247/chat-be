using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class Addmenuitemconfigarise : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"INSERT INTO dbo.Menus
                        (Code, Name, CodeParent, Note, NameEN, NameKO, IsParent)
                        VALUES(N'CAUHINH_PHATSINH', N'Cấu hình', N'DANHMUC', N'', NULL, NULL, 0);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
