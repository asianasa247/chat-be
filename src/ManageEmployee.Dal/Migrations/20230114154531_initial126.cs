using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial126 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "UsedCode",
            table: "FixedAssetUser",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Surcharges",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Type = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Surcharges", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Surcharges");

        migrationBuilder.DropColumn(
            name: "UsedCode",
            table: "FixedAssetUser");
    }
}
