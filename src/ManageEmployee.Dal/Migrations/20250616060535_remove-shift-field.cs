using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class removeshiftfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTimeThreshold",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "CheckOutTimeThreshold",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftEndAt",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftStartAt",
                table: "Shifts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CheckInTimeThreshold",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CheckOutTimeThreshold",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftEndAt",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftStartAt",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
