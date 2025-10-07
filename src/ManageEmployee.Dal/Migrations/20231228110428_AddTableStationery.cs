using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableStationery : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Stationeries",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Unit = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Stationeries", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StationeryExportItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StationeryId = table.Column<int>(type: "int", nullable: false),
                StationeryExportId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StationeryExportItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StationeryExports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DepartmentId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StationeryExports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StationeryImportItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StationeryId = table.Column<int>(type: "int", nullable: false),
                StationeryImportId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false),
                UnitPrice = table.Column<double>(type: "float", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StationeryImportItems", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "StationeryImports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StationeryImports", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Stationeries");

        migrationBuilder.DropTable(
            name: "StationeryExportItems");

        migrationBuilder.DropTable(
            name: "StationeryExports");

        migrationBuilder.DropTable(
            name: "StationeryImportItems");

        migrationBuilder.DropTable(
            name: "StationeryImports");
    }
}
