using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial16 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DocumentTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                table.PrimaryKey("PK_DocumentTypes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DocumentTypes");
    }
}
