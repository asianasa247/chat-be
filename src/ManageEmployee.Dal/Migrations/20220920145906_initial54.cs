using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial54 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreateAt",
            table: "Customers",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreateAt",
            table: "Customers");
    }
}
