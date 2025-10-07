using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial68 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Inventory",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Account = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                AccountName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Warehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                WarehouseName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Detail1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DetailName1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Detail2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DetailName2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Image1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                InputQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                OutputQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CloseQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CloseQuantityReal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Inventory", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Inventory");
    }
}
