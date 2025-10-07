using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableEventCustomerFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventCustomers_Customers_CustomerId",
                table: "EventCustomers");

            migrationBuilder.DropIndex(
                name: "IX_EventCustomers_CustomerId",
                table: "EventCustomers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "EventCustomers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "EventCustomers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventCustomers_CustomerId",
                table: "EventCustomers",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCustomers_Customers_CustomerId",
                table: "EventCustomers",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
