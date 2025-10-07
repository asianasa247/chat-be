using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class AddColumnPaymentAtIntoTableOrder : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "AdultQuantity",
            table: "OrderDetail",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ChildrenQuantity",
            table: "OrderDetail",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Broker",
            table: "Order",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "FromAt",
            table: "Order",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "Identifier",
            table: "Order",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsPayment",
            table: "Order",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "PaymentAt",
            table: "Order",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "ToAt",
            table: "Order",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AdultQuantity",
            table: "OrderDetail");

        migrationBuilder.DropColumn(
            name: "ChildrenQuantity",
            table: "OrderDetail");

        migrationBuilder.DropColumn(
            name: "Broker",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "FromAt",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "Identifier",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "IsPayment",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "PaymentAt",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "ToAt",
            table: "Order");
    }
}
