using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial22 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InOutHistories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TargetId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TimeIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                TimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                SymbolId = table.Column<int>(type: "int", nullable: false),
                CheckInMethod = table.Column<int>(type: "int", nullable: false),
                Checked = table.Column<bool>(type: "bit", nullable: false),
                IsOverTime = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InOutHistories", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InOutHistories");
    }
}
