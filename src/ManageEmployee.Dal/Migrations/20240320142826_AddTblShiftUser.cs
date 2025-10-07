using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblShiftUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ShiftUserDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                ShiftUserId = table.Column<int>(type: "int", nullable: false),
                TargetId = table.Column<int>(type: "int", nullable: true),
                Day1 = table.Column<int>(type: "int", nullable: true),
                Day2 = table.Column<int>(type: "int", nullable: true),
                Day3 = table.Column<int>(type: "int", nullable: true),
                Day4 = table.Column<int>(type: "int", nullable: true),
                Day5 = table.Column<int>(type: "int", nullable: true),
                Day6 = table.Column<int>(type: "int", nullable: true),
                Day7 = table.Column<int>(type: "int", nullable: true),
                Day8 = table.Column<int>(type: "int", nullable: true),
                Day9 = table.Column<int>(type: "int", nullable: true),
                Day10 = table.Column<int>(type: "int", nullable: true),
                Day11 = table.Column<int>(type: "int", nullable: true),
                Day12 = table.Column<int>(type: "int", nullable: true),
                Day13 = table.Column<int>(type: "int", nullable: true),
                Day14 = table.Column<int>(type: "int", nullable: true),
                Day15 = table.Column<int>(type: "int", nullable: true),
                Day16 = table.Column<int>(type: "int", nullable: true),
                Day17 = table.Column<int>(type: "int", nullable: true),
                Day18 = table.Column<int>(type: "int", nullable: true),
                Day19 = table.Column<int>(type: "int", nullable: true),
                Day20 = table.Column<int>(type: "int", nullable: true),
                Day21 = table.Column<int>(type: "int", nullable: true),
                Day22 = table.Column<int>(type: "int", nullable: true),
                Day23 = table.Column<int>(type: "int", nullable: true),
                Day24 = table.Column<int>(type: "int", nullable: true),
                Day25 = table.Column<int>(type: "int", nullable: true),
                Day26 = table.Column<int>(type: "int", nullable: true),
                Day27 = table.Column<int>(type: "int", nullable: true),
                Day28 = table.Column<int>(type: "int", nullable: true),
                Day29 = table.Column<int>(type: "int", nullable: true),
                Day30 = table.Column<int>(type: "int", nullable: true),
                Day31 = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShiftUserDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ShiftUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Month = table.Column<int>(type: "int", nullable: false),
                Year = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShiftUsers", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ShiftUserDetails");

        migrationBuilder.DropTable(
            name: "ShiftUsers");
    }
}
