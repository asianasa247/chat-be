using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial78 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "P_Leave",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Fromdt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Todt = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsLicensed = table.Column<bool>(type: "bit", nullable: false),
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
                table.PrimaryKey("PK_P_Leave", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_Procedure",
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
                table.PrimaryKey("PK_P_Procedure", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_ProcedureStatus",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                P_StatusId = table.Column<int>(type: "int", nullable: false),
                P_ProcedureId = table.Column<int>(type: "int", nullable: false),
                P_StatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_ProcedureStatus", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_ProcedureStatusRole",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                P_ProcedureStatusId = table.Column<int>(type: "int", nullable: false),
                RoleId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_ProcedureStatusRole", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_SalaryAdvance",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: true),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                P_ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                P_ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                table.PrimaryKey("PK_P_SalaryAdvance", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_SalaryAdvance_Item",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                P_SalaryAdvanceId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                BranchId = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_SalaryAdvance_Item", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_Status",
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
                table.PrimaryKey("PK_P_Status", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "P_Leave");

        migrationBuilder.DropTable(
            name: "P_Procedure");

        migrationBuilder.DropTable(
            name: "P_ProcedureStatus");

        migrationBuilder.DropTable(
            name: "P_ProcedureStatusRole");

        migrationBuilder.DropTable(
            name: "P_SalaryAdvance");

        migrationBuilder.DropTable(
            name: "P_SalaryAdvance_Item");

        migrationBuilder.DropTable(
            name: "P_Status");
    }
}
