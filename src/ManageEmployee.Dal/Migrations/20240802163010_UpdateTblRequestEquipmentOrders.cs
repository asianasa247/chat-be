using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateTblRequestEquipmentOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RequestEquipmentCode",
            table: "RequestEquipmentOrders",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "RequestEquipmentId",
            table: "RequestEquipmentOrders",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RequestEquipmentCode",
            table: "RequestEquipmentOrders");

        migrationBuilder.DropColumn(
            name: "RequestEquipmentId",
            table: "RequestEquipmentOrders");
    }
}
