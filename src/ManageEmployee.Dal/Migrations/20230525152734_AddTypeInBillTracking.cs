using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTypeInBillTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "BillTrackings",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Type",
            table: "BillTrackings",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Type",
            table: "BillTrackings");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "BillTrackings",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(1000)",
            oldMaxLength: 1000,
            oldNullable: true);
    }
}
