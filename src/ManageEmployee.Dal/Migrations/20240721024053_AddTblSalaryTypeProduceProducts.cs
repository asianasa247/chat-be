using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblSalaryTypeProduceProducts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ManufactureOrderCode",
            table: "ProduceProducts",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ManufactureOrderId",
            table: "ProduceProducts",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "SalaryTypeProduceProducts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProduceProductId = table.Column<int>(type: "int", nullable: false),
                ProduceProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SalaryTypeId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false),
                UserIdStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalaryTypeProduceProducts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SalaryTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                AmountSpent = table.Column<double>(type: "float", nullable: false),
                AmountSpentMonthly = table.Column<double>(type: "float", nullable: false),
                AmountAtTheEndYear = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalaryTypes", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SalaryTypeProduceProducts");

        migrationBuilder.DropTable(
            name: "SalaryTypes");

        migrationBuilder.DropColumn(
            name: "ManufactureOrderCode",
            table: "ProduceProducts");

        migrationBuilder.DropColumn(
            name: "ManufactureOrderId",
            table: "ProduceProducts");
    }
}
