using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RemoveColumnStatusInTblOrder : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Status",
            table: "OrderProduceProducts");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Status",
            table: "OrderProduceProducts",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
