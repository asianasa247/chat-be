using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class CreateTimeKeepingInOutLoggingTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TimeKeepingInOutLogging",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Traces = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TimeKeepingInOutLogging", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TimeKeepingInOutLogging");
    }
}
