using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class AddTableAmenityType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Amenities",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                AmenityTypeId = table.Column<int>(type: "int", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Amenities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AmenityTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AmenityTypes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Amenities");

        migrationBuilder.DropTable(
            name: "AmenityTypes");
    }
}
