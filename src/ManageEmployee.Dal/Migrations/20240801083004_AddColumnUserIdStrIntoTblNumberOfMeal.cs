using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnUserIdStrIntoTblNumberOfMeal : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserIdStr",
            table: "NumberOfMealDetails");

        migrationBuilder.AddColumn<string>(
            name: "UserIdStr",
            table: "NumberOfMeals",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserIdStr",
            table: "NumberOfMeals");

        migrationBuilder.AddColumn<string>(
            name: "UserIdStr",
            table: "NumberOfMealDetails",
            type: "nvarchar(max)",
            nullable: true);
    }
}
