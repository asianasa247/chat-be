using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddColumnBranchIdIntoBillDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "BillDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "BillDetailRefunds",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "BillDetails");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "BillDetailRefunds");
        }
    }
}
