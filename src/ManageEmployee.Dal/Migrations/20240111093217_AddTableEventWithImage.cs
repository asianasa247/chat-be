using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableEventWithImage : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventWithImages",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Order = table.Column<int>(type: "int", nullable: false),
                FileLink = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                LinkDriver = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventWithImages", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EventWithImages");
    }
}
