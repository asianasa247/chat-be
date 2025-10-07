using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class RenameTableAmenityToRoomCOnfig : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Name",
            table: "AmenityTypes",
            newName: "Type");

        migrationBuilder.RenameColumn(
            name: "Name",
            table: "Amenities",
            newName: "NameVn");

        migrationBuilder.AddColumn<string>(
            name: "NameEn",
            table: "AmenityTypes",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NameKo",
            table: "AmenityTypes",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NameVn",
            table: "AmenityTypes",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NameEn",
            table: "Amenities",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NameKo",
            table: "Amenities",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NameEn",
            table: "AmenityTypes");

        migrationBuilder.DropColumn(
            name: "NameKo",
            table: "AmenityTypes");

        migrationBuilder.DropColumn(
            name: "NameVn",
            table: "AmenityTypes");

        migrationBuilder.DropColumn(
            name: "NameEn",
            table: "Amenities");

        migrationBuilder.DropColumn(
            name: "NameKo",
            table: "Amenities");

        migrationBuilder.RenameColumn(
            name: "Type",
            table: "AmenityTypes",
            newName: "Name");

        migrationBuilder.RenameColumn(
            name: "NameVn",
            table: "Amenities",
            newName: "Name");
    }
}
