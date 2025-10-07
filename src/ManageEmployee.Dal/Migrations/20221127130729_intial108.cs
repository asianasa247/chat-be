using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial108 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PriceList",
            table: "Categories");

        migrationBuilder.AddColumn<bool>(
            name: "IsShowWeb",
            table: "Categories",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsShowWeb",
            table: "Categories");

        migrationBuilder.AddColumn<string>(
            name: "PriceList",
            table: "Categories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
