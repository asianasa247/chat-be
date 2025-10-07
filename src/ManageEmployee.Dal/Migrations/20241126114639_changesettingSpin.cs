using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class changesettingSpin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimePerSpin",
                table: "SettingsSpins");

            migrationBuilder.AddColumn<int>(
                name: "TimeStartPerSpin",
                table: "SettingsSpins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeStopPerSpin",
                table: "SettingsSpins",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStartPerSpin",
                table: "SettingsSpins");

            migrationBuilder.DropColumn(
                name: "TimeStopPerSpin",
                table: "SettingsSpins");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimePerSpin",
                table: "SettingsSpins",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
