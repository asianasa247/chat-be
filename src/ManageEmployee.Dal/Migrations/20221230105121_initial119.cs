using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial119 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MainColors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Color = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MainColors", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MainColors");
    }
}
