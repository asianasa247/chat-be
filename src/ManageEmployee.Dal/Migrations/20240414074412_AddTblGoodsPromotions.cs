using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblGoodsPromotions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GoodsPromotions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Value = table.Column<double>(type: "float", nullable: false),
                FromAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ToAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                FileLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CustomerNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsPromotions", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodsPromotions");
    }
}
