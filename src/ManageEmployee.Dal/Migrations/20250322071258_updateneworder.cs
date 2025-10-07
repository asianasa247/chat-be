using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class updateneworder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TipMonney",
                table: "EmployeeByOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isPercent",
                table: "Commissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipMonney",
                table: "EmployeeByOrders");

            migrationBuilder.DropColumn(
                name: "isPercent",
                table: "Commissions");
        }
    }
}
