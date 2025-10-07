using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnIsCanceledIntoTblOrderProduceProducts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsCanceled",
            table: "OrderProduceProducts",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "IsDelivered",
            table: "OrderProduceProducts",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsCanceled",
            table: "OrderProduceProducts");

        migrationBuilder.DropColumn(
            name: "IsDelivered",
            table: "OrderProduceProducts");
    }
}
