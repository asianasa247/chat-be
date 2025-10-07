using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class addtblConvertProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConvertProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ConvertQuantity = table.Column<int>(type: "int", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Warehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    WarehouseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DetailName1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DetailName2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OppositeAccount = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    OppositeAccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OppositeWarehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    OppositeWarehouseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OppositeDetail1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OppositeDetailName1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OppositeDetail2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OppositeDetailName2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertProducts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertProducts");
        }
    }
}
