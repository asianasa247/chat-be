using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddEffectiveFromToUserSalaryVersionMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "EffectiveFrom",
            table: "SalaryUserVersions",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "EffectiveTo",
            table: "SalaryUserVersions",
            type: "datetime2",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "EffectiveFrom",
            table: "SalaryUserVersions");

        migrationBuilder.DropColumn(
            name: "EffectiveTo",
            table: "SalaryUserVersions");
    }
}
