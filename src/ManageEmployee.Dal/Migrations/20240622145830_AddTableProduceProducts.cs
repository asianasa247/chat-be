using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableProduceProducts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProduceProductId = table.Column<int>(type: "int", nullable: false),
                GoodsId = table.Column<int>(type: "int", nullable: false),
                QuantityRequired = table.Column<double>(type: "float", nullable: false),
                QuantityReal = table.Column<double>(type: "float", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CarId = table.Column<int>(type: "int", nullable: true),
                CarName = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProduceProducts", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProduceProductDetails");

        migrationBuilder.DropTable(
            name: "ProduceProducts");
    }
}
