using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddFileLinkInTblCar : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileName",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "FileUrl",
            table: "Cars");

        migrationBuilder.AddColumn<string>(
            name: "FileLink",
            table: "Cars",
            type: "nvarchar(1000)",
            maxLength: 1000,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileLink",
            table: "Cars");

        migrationBuilder.AddColumn<string>(
            name: "FileName",
            table: "Cars",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileUrl",
            table: "Cars",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);
    }
}
