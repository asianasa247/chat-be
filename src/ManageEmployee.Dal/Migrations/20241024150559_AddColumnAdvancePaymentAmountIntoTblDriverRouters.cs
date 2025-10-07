using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddColumnAdvancePaymentAmountIntoTblDriverRouters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AdvancePaymentAmount",
                table: "DriverRouters",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelAmount",
                table: "DriverRouters",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvancePaymentAmount",
                table: "DriverRouters");

            migrationBuilder.DropColumn(
                name: "FuelAmount",
                table: "DriverRouters");
        }
    }
}
