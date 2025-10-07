using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblDriverRouters : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DriverRouterDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DriverRouterId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PoliceCheckPointId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DriverRouterDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DriverRouters",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PetrolConsumptionId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DriverRouters", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DriverRouterDetails");

        migrationBuilder.DropTable(
            name: "DriverRouters");
    }
}
