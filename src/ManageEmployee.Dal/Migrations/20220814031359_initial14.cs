using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial14 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsFloor",
            table: "DeskFloors",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsFloor",
            table: "DeskFloors");
    }
}
