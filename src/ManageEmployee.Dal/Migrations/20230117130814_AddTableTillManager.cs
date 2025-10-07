using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableTillManager : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TillManagers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FromAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                FromAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                ToAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ToAmountAuto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsDifferentMoney = table.Column<bool>(type: "bit", nullable: false),
                AmountDifferent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: false),
                IsFinish = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TillManagers", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TillManagers");
    }
}
