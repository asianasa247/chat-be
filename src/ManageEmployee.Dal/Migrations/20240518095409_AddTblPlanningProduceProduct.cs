using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblPlanningProduceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProduceProductDetails");

        migrationBuilder.DropTable(
            name: "ProduceProducts");

        migrationBuilder.RenameColumn(
            name: "UpdatedBy",
            table: "OrderProduceProducts",
            newName: "UserUpdated");

        migrationBuilder.RenameColumn(
            name: "CreatedBy",
            table: "OrderProduceProducts",
            newName: "UserCreated");

        migrationBuilder.CreateTable(
            name: "PlanningProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CarId = table.Column<int>(type: "int", nullable: true),
                ProduceProductId = table.Column<int>(type: "int", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PlanningProduceProductDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PlanningProduceProducts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                StatusId = table.Column<int>(type: "int", nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                table.PrimaryKey("PK_PlanningProduceProducts", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PlanningProduceProductDetails");

        migrationBuilder.DropTable(
            name: "PlanningProduceProducts");

        migrationBuilder.RenameColumn(
            name: "UserUpdated",
            table: "OrderProduceProducts",
            newName: "UpdatedBy");

        migrationBuilder.RenameColumn(
            name: "UserCreated",
            table: "OrderProduceProducts",
            newName: "CreatedBy");

        migrationBuilder.CreateTable(
            name: "ProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                ProduceProductId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProduceProductDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProduceProducts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<int>(type: "int", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                StatusId = table.Column<int>(type: "int", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProduceProducts", x => x.Id);
            });
    }
}
