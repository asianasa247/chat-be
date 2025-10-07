using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial28 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Certificates",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Description = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                CompanyId = table.Column<int>(type: "int", nullable: true),
                Status = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Certificates", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Certificates");
    }
}
