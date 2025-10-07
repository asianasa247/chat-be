using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblConfigurationUsers : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Employees");

        migrationBuilder.CreateTable(
            name: "ConfigurationUsers",
            columns: table => new
            {
                Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Data = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                UserId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConfigurationUsers", x => x.Key);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ConfigurationUsers");

        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IdentityNumber = table.Column<double>(type: "float", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PlaceOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PlaceResidence = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
            });
    }
}
