using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnTotalAMountBuyIntoTblCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "isPublish",
            table: "Categories",
            newName: "IsPublish");

        migrationBuilder.RenameColumn(
            name: "isEnableDelete",
            table: "Categories",
            newName: "IsEnableDelete");

        migrationBuilder.AddColumn<double>(
            name: "TotalAmountBuy",
            table: "Categories",
            type: "float",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TotalAmountBuy",
            table: "Categories");

        migrationBuilder.RenameColumn(
            name: "IsPublish",
            table: "Categories",
            newName: "isPublish");

        migrationBuilder.RenameColumn(
            name: "IsEnableDelete",
            table: "Categories",
            newName: "isEnableDelete");
    }
}
