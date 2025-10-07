using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCarIdIntoTblWarehouseProduceProductDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CarId",
            table: "WarehouseProduceProductDetails",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "ManufactureOrderDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ManufactureOrderId = table.Column<int>(type: "int", nullable: false),
                WarehousePlanningProduceProductId = table.Column<int>(type: "int", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ManufactureOrderDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ManufactureOrders",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                UserId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ManufactureOrders", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ManufactureOrderDetails");

        migrationBuilder.DropTable(
            name: "ManufactureOrders");

        migrationBuilder.DropColumn(
            name: "CarId",
            table: "WarehouseProduceProductDetails");
    }
}
