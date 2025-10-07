using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableCar : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Cars",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LicensePlates = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Km = table.Column<double>(type: "float", nullable: false),
                Capacity = table.Column<double>(type: "float", nullable: false),
                RegistrationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                InsuranceAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cars", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PetrolConsumptions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                CarId = table.Column<int>(type: "int", nullable: false),
                PetroPrice = table.Column<double>(type: "float", nullable: false),
                KmFrom = table.Column<double>(type: "float", nullable: false),
                KmTo = table.Column<double>(type: "float", nullable: false),
                LocationFrom = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                LocationTo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                AdvanceAmount = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PetrolConsumptions", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Cars");

        migrationBuilder.DropTable(
            name: "PetrolConsumptions");
    }
}
