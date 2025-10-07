using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial53 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Img",
            table: "Sliders",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "DeskFloors",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Order",
            table: "DeskFloors");

        migrationBuilder.AlterColumn<string>(
            name: "Img",
            table: "Sliders",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }
}
