using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RemoveMenuWebInTableGoods : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MenuWeb",
            table: "Goods");

        migrationBuilder.AddColumn<string>(
            name: "Type",
            table: "SendMails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Type",
            table: "SendMails");

        migrationBuilder.AddColumn<string>(
            name: "MenuWeb",
            table: "Goods",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
