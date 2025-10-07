using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial61 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "TypeWorkId",
            table: "UserTasks",
            type: "int",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "TypeWorkId",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");
    }
}
