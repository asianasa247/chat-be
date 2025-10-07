using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblRoadRoute : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "RoadRouteId",
            table: "PetrolConsumptions",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "PetrolConsumptionPoliceCheckPoints",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PetrolConsumptionId = table.Column<int>(type: "int", nullable: false),
                PoliceCheckPointName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Amount = table.Column<double>(type: "float", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<int>(type: "int", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PetrolConsumptionPoliceCheckPoints", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PoliceCheckPoints",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Amount = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PoliceCheckPoints", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RoadRoutes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                RoadRouteDetail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoadRoutes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PetrolConsumptionPoliceCheckPoints");

        migrationBuilder.DropTable(
            name: "PoliceCheckPoints");

        migrationBuilder.DropTable(
            name: "RoadRoutes");

        migrationBuilder.DropColumn(
            name: "RoadRouteId",
            table: "PetrolConsumptions");
    }
}
