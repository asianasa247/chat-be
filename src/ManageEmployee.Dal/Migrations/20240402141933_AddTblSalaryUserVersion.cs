using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblSalaryUserVersion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SalaryUserVersions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                UserFullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ContractTypeId = table.Column<int>(type: "int", nullable: true),
                SalaryFrom = table.Column<double>(type: "float", nullable: true),
                SalaryTo = table.Column<double>(type: "float", nullable: true),
                SocialInsuranceSalary = table.Column<double>(type: "float", nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalaryUserVersions", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SalaryUserVersions");
    }
}
