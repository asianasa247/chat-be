using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RenameRoleAdminToSuperAdmin : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"UPDATE UserRoles 
                                    set Code  = 'SUPER_ADMIN' 
                                    WHERE  Code  = 'ADMIN'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {

    }
}
