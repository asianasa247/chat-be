using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddNewColumnInTargetTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "AllowedRadius",
            table: "Targets",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<string>(
            name: "IpAddress",
            table: "Targets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "LatitudePoint",
            table: "Targets",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<double>(
            name: "LongitudePoint",
            table: "Targets",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AllowedRadius",
            table: "Targets");

        migrationBuilder.DropColumn(
            name: "IpAddress",
            table: "Targets");

        migrationBuilder.DropColumn(
            name: "LatitudePoint",
            table: "Targets");

        migrationBuilder.DropColumn(
            name: "LongitudePoint",
            table: "Targets");
    }
}
