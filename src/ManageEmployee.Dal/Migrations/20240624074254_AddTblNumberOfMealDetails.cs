using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblNumberOfMealDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "NumberOfMealDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                TimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true),
                UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                QuantityAdd = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NumberOfMealDetails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "NumberOfMealDetails");
    }
}
