using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial65 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AccountPays",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AccountPays", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GoodDetails",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Warehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                AccountParent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AccountNameParent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail1Parent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName1Parent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Detail2Parent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DetailName2Parent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                WarehouseParent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                GoodID = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodDetails", x => x.ID);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AccountPays");

        migrationBuilder.DropTable(
            name: "GoodDetails");
    }
}
