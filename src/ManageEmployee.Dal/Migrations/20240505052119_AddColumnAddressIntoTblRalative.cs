using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnAddressIntoTblRalative : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "DistrictId",
            table: "Relatives",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "NativeDistrictId",
            table: "Relatives",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "NativeProvinceId",
            table: "Relatives",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "NativeWardId",
            table: "Relatives",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ProvinceId",
            table: "Relatives",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "WardId",
            table: "Relatives",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DistrictId",
            table: "Relatives");

        migrationBuilder.DropColumn(
            name: "NativeDistrictId",
            table: "Relatives");

        migrationBuilder.DropColumn(
            name: "NativeProvinceId",
            table: "Relatives");

        migrationBuilder.DropColumn(
            name: "NativeWardId",
            table: "Relatives");

        migrationBuilder.DropColumn(
            name: "ProvinceId",
            table: "Relatives");

        migrationBuilder.DropColumn(
            name: "WardId",
            table: "Relatives");
    }
}
