using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial71 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AttachVoucher",
            table: "FixedAssets",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "BuyDate",
            table: "FixedAssets",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "Quantity",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "UnitPrice",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AttachVoucher",
            table: "FixedAsset242",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "BuyDate",
            table: "FixedAsset242",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "Quantity",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "UnitPrice",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Allowances",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CompanyId = table.Column<int>(type: "int", nullable: true),
                Status = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Allowances", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AllowanceUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                AllowanceId = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AllowanceUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "InvoiceDeclarations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TemplateSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceSymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TotalInvoice = table.Column<int>(type: "int", nullable: true),
                FromOpening = table.Column<int>(type: "int", nullable: true),
                ToOpening = table.Column<int>(type: "int", nullable: true),
                FromArising = table.Column<int>(type: "int", nullable: true),
                ToArising = table.Column<int>(type: "int", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InvoiceDeclarations", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Allowances");

        migrationBuilder.DropTable(
            name: "AllowanceUsers");

        migrationBuilder.DropTable(
            name: "InvoiceDeclarations");

        migrationBuilder.DropColumn(
            name: "AttachVoucher",
            table: "FixedAssets");

        migrationBuilder.DropColumn(
            name: "BuyDate",
            table: "FixedAssets");

        migrationBuilder.DropColumn(
            name: "Quantity",
            table: "FixedAssets");

        migrationBuilder.DropColumn(
            name: "UnitPrice",
            table: "FixedAssets");

        migrationBuilder.DropColumn(
            name: "AttachVoucher",
            table: "FixedAsset242");

        migrationBuilder.DropColumn(
            name: "BuyDate",
            table: "FixedAsset242");

        migrationBuilder.DropColumn(
            name: "Quantity",
            table: "FixedAsset242");

        migrationBuilder.DropColumn(
            name: "UnitPrice",
            table: "FixedAsset242");
    }
}
