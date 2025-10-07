using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial76 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TamUngLuong",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: true),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                QuyTrinhTrangThaiId = table.Column<int>(type: "int", nullable: true),
                QuyTrinhTrangThaiName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
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
                TamUngLuongId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TamUngLuong_Item", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TamUngLuong");

        migrationBuilder.DropTable(
            name: "TamUngLuong_Item");
    }
}
