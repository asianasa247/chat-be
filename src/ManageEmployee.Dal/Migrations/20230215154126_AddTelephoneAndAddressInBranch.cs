using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTelephoneAndAddressInBranch : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Address",
            table: "Branchs",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "TelephoneNumber",
            table: "Branchs",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Address",
            table: "Branchs");

        migrationBuilder.DropColumn(
            name: "TelephoneNumber",
            table: "Branchs");
    }
}
