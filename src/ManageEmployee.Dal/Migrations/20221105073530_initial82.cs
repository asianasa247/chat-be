using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial82 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MenuKpis",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Value = table.Column<double>(type: "float", nullable: false),
                Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MenuKpis", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_Kpis",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true),
                P_ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                P_ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_Kpis", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MenuKpis");

        migrationBuilder.DropTable(
            name: "P_Kpis");
    }
}
