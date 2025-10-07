using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblProduceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OrderProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrderProduceProductId = table.Column<int>(type: "int", nullable: false),
                ProduceProductId = table.Column<int>(type: "int", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                QuantityRequired = table.Column<double>(type: "float", nullable: false),
                QuantityReal = table.Column<double>(type: "float", nullable: false),
                IsProduced = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderProduceProductDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OrderProduceProducts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedBy = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<int>(type: "int", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderProduceProducts", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                IsProduce = table.Column<bool>(type: "bit", nullable: false),
                ProduceProductId = table.Column<int>(type: "int", nullable: false),
                QuantityReal = table.Column<double>(type: "float", nullable: false),
                QuantityRequired = table.Column<double>(type: "float", nullable: false)
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
