using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial10 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DeskFloors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FloorId = table.Column<int>(type: "int", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsDesk = table.Column<bool>(type: "bit", nullable: false),
                NumberSeat = table.Column<int>(type: "int", nullable: false),
                Position = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                IsChoose = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeskFloors", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DeskFloors");
    }
}
