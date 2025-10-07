using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial39 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SalaryLevel",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PositionId = table.Column<int>(type: "int", nullable: false),
                PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SalaryCost = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Coefficient = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalaryLevel", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Subsidizes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                SortCode = table.Column<int>(type: "int", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Subsidizes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserSubsidizes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SubsidizeId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Values = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserSubsidizes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SalaryLevel");

        migrationBuilder.DropTable(
            name: "Subsidizes");

        migrationBuilder.DropTable(
            name: "UserSubsidizes");
    }
}
