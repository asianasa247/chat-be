using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateTblRoadRoutes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "RoadRoutes",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "PoliceCheckPointIdStr",
            table: "RoadRoutes",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "RoadRoutes",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "UserCreated",
            table: "RoadRoutes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "UserUpdated",
            table: "RoadRoutes",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "RoadRoutes");

        migrationBuilder.DropColumn(
            name: "PoliceCheckPointIdStr",
            table: "RoadRoutes");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "RoadRoutes");

        migrationBuilder.DropColumn(
            name: "UserCreated",
            table: "RoadRoutes");

        migrationBuilder.DropColumn(
            name: "UserUpdated",
            table: "RoadRoutes");
    }
}
