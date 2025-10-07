using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCavetIntoTblCar : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AccumulatorNumber",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Cavet",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "KmChangeOil",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "KmTestBrake",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "KmTestCone",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Owner",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TyreNumber",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "VehicleBadgeAt",
            table: "Cars",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "VehicleBadgeNumber",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "VehicleLoad",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AccumulatorNumber",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "Cavet",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "KmChangeOil",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "KmTestBrake",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "KmTestCone",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "Owner",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "TyreNumber",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "VehicleBadgeAt",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "VehicleBadgeNumber",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "VehicleLoad",
            table: "Cars");
    }
}
