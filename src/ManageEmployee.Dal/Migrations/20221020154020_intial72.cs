using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial72 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsPrintBill",
            table: "Bills",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "QuyTrinhs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuyTrinhs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "QuyTrinhTrangThaiRoles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                QuyTrinhTrangThaiId = table.Column<int>(type: "int", nullable: false),
                RoleId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuyTrinhTrangThaiRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "QuyTrinhTrangThais",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TrangThaiId = table.Column<int>(type: "int", nullable: false),
                QuyTrinhId = table.Column<int>(type: "int", nullable: false),
                TrangThaiName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuyTrinhTrangThais", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TrangThais",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TrangThais", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "QuyTrinhs");

        migrationBuilder.DropTable(
            name: "QuyTrinhTrangThaiRoles");

        migrationBuilder.DropTable(
            name: "QuyTrinhTrangThais");

        migrationBuilder.DropTable(
            name: "TrangThais");

        migrationBuilder.DropColumn(
            name: "IsPrintBill",
            table: "Bills");
    }
}
