using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial86 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BranchId",
            table: "P_Kpis");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "P_Kpis");

        migrationBuilder.CreateTable(
            name: "P_Kpi_Items",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                P_KpiId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: true),
                PointKpi = table.Column<double>(type: "float", nullable: true),
                Point = table.Column<double>(type: "float", nullable: true),
                Percent = table.Column<double>(type: "float", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_Kpi_Items", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "P_Kpi_Items");

        migrationBuilder.AddColumn<int>(
            name: "BranchId",
            table: "P_Kpis",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserId",
            table: "P_Kpis",
            type: "int",
            nullable: true);
    }
}
