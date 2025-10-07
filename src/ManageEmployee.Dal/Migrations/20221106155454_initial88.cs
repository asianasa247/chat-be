using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial88 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Inventory",
            table: "GoodWarehouses",
            newName: "Quantity");

        migrationBuilder.AddColumn<bool>(
            name: "isFinish",
            table: "P_SalaryAdvance",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "isFinish",
            table: "P_Leave",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "isFinish",
            table: "P_Kpis",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateExpiration",
            table: "Inventory",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "isCheck",
            table: "Inventory",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "GoodWarehouseExport",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                GoodWarehouseId = table.Column<int>(type: "int", nullable: false),
                BillId = table.Column<int>(type: "int", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodWarehouseExport", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_Inventories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProcedureNumber = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                P_ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                P_ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                isFinish = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_Inventories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "P_Inventory_Items",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                P_InventoryId = table.Column<int>(type: "int", nullable: false),
                Account = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                AccountName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Warehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                WarehouseName = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Detail1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DetailName1 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Detail2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                DetailName2 = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                InputQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                OutputQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CloseQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CloseQuantityReal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_Inventory_Items", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodWarehouseExport");

        migrationBuilder.DropTable(
            name: "P_Inventories");

        migrationBuilder.DropTable(
            name: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "isFinish",
            table: "P_SalaryAdvance");

        migrationBuilder.DropColumn(
            name: "isFinish",
            table: "P_Leave");

        migrationBuilder.DropColumn(
            name: "isFinish",
            table: "P_Kpis");

        migrationBuilder.DropColumn(
            name: "DateExpiration",
            table: "Inventory");

        migrationBuilder.DropColumn(
            name: "isCheck",
            table: "Inventory");

        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "GoodWarehouses",
            newName: "Inventory");
    }
}
