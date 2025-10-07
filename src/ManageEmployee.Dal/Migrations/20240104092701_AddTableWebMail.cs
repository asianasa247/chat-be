using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableWebMail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "WebMails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CustomerId = table.Column<int>(type: "int", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WebMails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WebMails");
    }
}
