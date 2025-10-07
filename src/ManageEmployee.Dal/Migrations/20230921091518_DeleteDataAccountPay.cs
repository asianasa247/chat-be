using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class DeleteDataAccountPay : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TaxVAT",
            table: "Goods");
        migrationBuilder.Sql("delete from AccountPays");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "TaxVAT",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true);
    }
}
