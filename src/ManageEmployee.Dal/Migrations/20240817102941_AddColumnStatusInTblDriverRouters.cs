using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnStatusInTblDriverRouters : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "LedgerProduceImports",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "LedgerProduceExports",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "DriverRouters",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "DriverRouterDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "LedgerProduceImports");

        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "LedgerProduceExports");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "DriverRouters");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "DriverRouterDetails");
    }
}
