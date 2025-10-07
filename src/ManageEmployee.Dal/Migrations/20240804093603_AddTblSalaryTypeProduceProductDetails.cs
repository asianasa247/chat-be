using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblSalaryTypeProduceProductDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserIdStr",
            table: "SalaryTypeProduceProducts");

        migrationBuilder.CreateTable(
            name: "SalaryTypeProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TargetId = table.Column<int>(type: "int", nullable: false),
                SalaryTypeProduceProductId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<double>(type: "float", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Percent = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalaryTypeProduceProductDetails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SalaryTypeProduceProductDetails");

        migrationBuilder.AddColumn<string>(
            name: "UserIdStr",
            table: "SalaryTypeProduceProducts",
            type: "nvarchar(max)",
            nullable: true);
    }
}
