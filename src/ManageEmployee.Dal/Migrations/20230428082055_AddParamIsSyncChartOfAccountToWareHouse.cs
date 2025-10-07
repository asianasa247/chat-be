using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddParamIsSyncChartOfAccountToWareHouse : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsSyncChartOfAccount",
            table: "Warehouses",
            type: "bit",
            nullable: false,
            defaultValue: true);

        migrationBuilder.CreateTable(
            name: "CustomerTaxInformationAccountants",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerTaxInformationId = table.Column<int>(type: "int", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerTaxInformationAccountants", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CustomerTaxInformationAccountants");

        migrationBuilder.DropColumn(
            name: "IsSyncChartOfAccount",
            table: "Warehouses");
    }
}
