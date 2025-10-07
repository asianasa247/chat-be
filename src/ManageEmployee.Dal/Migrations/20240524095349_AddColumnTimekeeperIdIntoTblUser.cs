using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnTimekeeperIdIntoTblUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "NumberOfMeals",
            table: "Users",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TimekeeperId",
            table: "Users",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NumberOfMeals",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "TimekeeperId",
            table: "Users");
    }
}
