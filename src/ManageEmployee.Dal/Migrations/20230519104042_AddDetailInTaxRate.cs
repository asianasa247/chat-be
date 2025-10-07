using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddDetailInTaxRate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreditFirstCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CreditSecondCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DebitFirstCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DebitSecondCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreditFirstCode",
            table: "TaxRates");

        migrationBuilder.DropColumn(
            name: "CreditSecondCode",
            table: "TaxRates");

        migrationBuilder.DropColumn(
            name: "DebitFirstCode",
            table: "TaxRates");

        migrationBuilder.DropColumn(
            name: "DebitSecondCode",
            table: "TaxRates");
    }
}
