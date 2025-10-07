using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial56 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MenuId",
            table: "MenuRoles");

        migrationBuilder.DropColumn(
            name: "UserRoleId",
            table: "MenuRoles");

        migrationBuilder.AddColumn<string>(
            name: "MenuCode",
            table: "MenuRoles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UserRoleCode",
            table: "MenuRoles",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Order",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalPriceDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalPricePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Tell = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Order", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OrderDetail",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrderId = table.Column<int>(type: "int", nullable: false),
                GoodId = table.Column<int>(type: "int", nullable: false),
                GoodDetailId = table.Column<int>(type: "int", nullable: true),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxVAT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderDetail", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Order");

        migrationBuilder.DropTable(
            name: "OrderDetail");

        migrationBuilder.DropColumn(
            name: "MenuCode",
            table: "MenuRoles");

        migrationBuilder.DropColumn(
            name: "UserRoleCode",
            table: "MenuRoles");

        migrationBuilder.AddColumn<int>(
            name: "MenuId",
            table: "MenuRoles",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserRoleId",
            table: "MenuRoles",
            type: "int",
            nullable: true);
    }
}
