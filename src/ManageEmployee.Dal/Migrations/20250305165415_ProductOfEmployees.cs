using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class ProductOfEmployees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceCount",
                table: "EmployeeByOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductOfEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    detail1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    detailName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    detail2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    detailName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accountName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOfEmployees", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOfEmployees");

            migrationBuilder.DropColumn(
                name: "ServiceCount",
                table: "EmployeeByOrders");
        }
    }
}
