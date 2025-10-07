using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblRequestEquipment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "RequestEquipmentDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RequestEquipmentId = table.Column<int>(type: "int", nullable: false),
                GoodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                GoodCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                GoodProducer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                GoodCatalog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                GoodUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Quantity = table.Column<double>(type: "float", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                GoodType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RequestEquipmentDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RequestEquipments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: false),
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
                table.PrimaryKey("PK_RequestEquipments", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "RequestEquipmentDetails");

        migrationBuilder.DropTable(
            name: "RequestEquipments");
    }
}
