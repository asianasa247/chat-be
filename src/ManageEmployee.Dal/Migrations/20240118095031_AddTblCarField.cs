using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblCarField : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AccumulatorNumber",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "Capacity",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "Cavet",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "InsuranceAt",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "Km",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "KmChangeOil",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "KmTestBrake",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "KmTestCone",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "Owner",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "RegistrationAt",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "TyreNumber",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "VehicleBadgeAt",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "VehicleBadgeNumber",
            table: "Cars");

        migrationBuilder.DropColumn(
            name: "VehicleLoad",
            table: "Cars");

        migrationBuilder.CreateTable(
            name: "CarFields",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CarId = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarFields", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CarFieldSetups",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CarId = table.Column<int>(type: "int", nullable: false),
                CarFieldId = table.Column<int>(type: "int", nullable: false),
                ValueNumber = table.Column<double>(type: "float", nullable: true),
                ValueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                FromAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ToAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                WarningAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarFieldSetups", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CarFields");

        migrationBuilder.DropTable(
            name: "CarFieldSetups");

        migrationBuilder.AddColumn<string>(
            name: "AccumulatorNumber",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "Capacity",
            table: "Cars",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<string>(
            name: "Cavet",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "InsuranceAt",
            table: "Cars",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "Km",
            table: "Cars",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<double>(
            name: "KmChangeOil",
            table: "Cars",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "KmTestBrake",
            table: "Cars",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "KmTestCone",
            table: "Cars",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Owner",
            table: "Cars",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "RegistrationAt",
            table: "Cars",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "TyreNumber",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "VehicleBadgeAt",
            table: "Cars",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "VehicleBadgeNumber",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "VehicleLoad",
            table: "Cars",
            type: "float",
            nullable: true);
    }
}
