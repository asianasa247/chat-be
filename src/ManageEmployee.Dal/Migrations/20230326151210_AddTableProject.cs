using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableProject : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LedgerErrorImports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LedgerId = table.Column<long>(type: "bigint", nullable: false),
                ErrorCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsInternal = table.Column<int>(type: "int", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerErrorImports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Projects",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Projects", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerErrorImports");

        migrationBuilder.DropTable(
            name: "Projects");
    }
}
