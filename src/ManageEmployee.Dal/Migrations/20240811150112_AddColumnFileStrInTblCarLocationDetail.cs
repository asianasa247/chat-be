using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnFileStrInTblCarLocationDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Content",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "CarLocationDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsImmediate",
            table: "AdvancePayments",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Content",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "CarLocationDetails");

        migrationBuilder.DropColumn(
            name: "IsImmediate",
            table: "AdvancePayments");
    }
}
