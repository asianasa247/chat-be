using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblInOutReports : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InOutReports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                Year = table.Column<int>(type: "int", nullable: false),
                Day1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day3 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day4 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day5 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day6 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day7 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day8 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day9 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day10 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day11 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day12 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day13 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day14 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day15 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day16 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day17 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day18 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day19 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day20 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day21 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day22 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day23 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day24 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day25 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day26 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day27 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day28 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day29 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day30 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Day31 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InOutReports", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InOutReports");
    }
}
