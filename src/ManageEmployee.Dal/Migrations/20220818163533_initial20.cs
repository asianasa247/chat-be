using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial20 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Descriptions",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DebitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Descriptions", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Descriptions");
    }
}
