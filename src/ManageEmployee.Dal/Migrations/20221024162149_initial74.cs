using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial74 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "Inventory",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AddColumn<string>(
            name: "OrginalVoucherNumber",
            table: "GoodWarehouses",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "IsoftHistory",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Order = table.Column<int>(type: "int", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IsoftHistory", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "IsoftHistory");

        migrationBuilder.DropColumn(
            name: "OrginalVoucherNumber",
            table: "GoodWarehouses");

        migrationBuilder.AlterColumn<long>(
            name: "Inventory",
            table: "GoodWarehouses",
            type: "bigint",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");
    }
}
