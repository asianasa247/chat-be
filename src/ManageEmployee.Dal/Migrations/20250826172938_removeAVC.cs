using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class removeAVC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountQD",
                table: "Ledgers");

            migrationBuilder.DropColumn(
                name: "OrginalCurrencyQD",
                table: "Ledgers");

            migrationBuilder.DropColumn(
                name: "QuantityQD",
                table: "Ledgers");

            migrationBuilder.DropColumn(
                name: "UnitPriceQD",
                table: "Ledgers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountQD",
                table: "Ledgers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrginalCurrencyQD",
                table: "Ledgers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityQD",
                table: "Ledgers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitPriceQD",
                table: "Ledgers",
                type: "int",
                nullable: true);
        }
    }
}
