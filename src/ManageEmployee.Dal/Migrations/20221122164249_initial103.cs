using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial103 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "DeleteNumber",
            table: "InvoiceDeclarations",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DeleteNumberItem",
            table: "InvoiceDeclarations",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "FromUsed",
            table: "InvoiceDeclarations",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ToUsed",
            table: "InvoiceDeclarations",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UsedNumber",
            table: "InvoiceDeclarations",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DeleteNumber",
            table: "InvoiceDeclarations");

        migrationBuilder.DropColumn(
            name: "DeleteNumberItem",
            table: "InvoiceDeclarations");

        migrationBuilder.DropColumn(
            name: "FromUsed",
            table: "InvoiceDeclarations");

        migrationBuilder.DropColumn(
            name: "ToUsed",
            table: "InvoiceDeclarations");

        migrationBuilder.DropColumn(
            name: "UsedNumber",
            table: "InvoiceDeclarations");
    }
}
