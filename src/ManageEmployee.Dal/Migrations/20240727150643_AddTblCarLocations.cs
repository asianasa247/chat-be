using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblCarLocations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CarLocationDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CarLocationId = table.Column<int>(type: "int", nullable: false),
                LicensePlates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PlanInprogress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PlanExpected = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarLocationDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CarLocations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarLocations", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CarLocationDetails");

        migrationBuilder.DropTable(
            name: "CarLocations");
    }
}
