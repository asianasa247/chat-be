using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddContactIntoOrder : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "Date",
            table: "Order",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PaymentMethod",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Promotion",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Date",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "Email",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "PaymentMethod",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "PhoneNumber",
            table: "Order");

        migrationBuilder.DropColumn(
            name: "Promotion",
            table: "Order");
    }
}
