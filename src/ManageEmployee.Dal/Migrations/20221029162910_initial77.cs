using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial77 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "QuyTrinhs");

        migrationBuilder.DropTable(
            name: "QuyTrinhTrangThaiRoles");

        migrationBuilder.DropTable(
            name: "QuyTrinhTrangThais");

        migrationBuilder.DropTable(
            name: "TamUngLuong");

        migrationBuilder.DropTable(
            name: "TamUngLuong_Item");

        migrationBuilder.DropTable(
            name: "TrangThais");

        migrationBuilder.DropTable(
            name: "XinNghiPheps");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
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
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                QuyTrinhTrangThaiId = table.Column<int>(type: "int", nullable: false),
                RoleId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true)
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
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                QuyTrinhId = table.Column<int>(type: "int", nullable: false),
                TrangThaiId = table.Column<int>(type: "int", nullable: false),
                TrangThaiName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuyTrinhTrangThais", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TamUngLuong",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BranchId = table.Column<int>(type: "int", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                QuyTrinhTrangThaiId = table.Column<int>(type: "int", nullable: true),
                QuyTrinhTrangThaiName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TamUngLuong", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TamUngLuong_Item",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BranchId = table.Column<int>(type: "int", nullable: false),
                TamUngLuongId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TamUngLuong_Item", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TrangThais",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Type = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TrangThais", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "XinNghiPheps",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_XinNghiPheps", x => x.Id);
            });
    }
}
