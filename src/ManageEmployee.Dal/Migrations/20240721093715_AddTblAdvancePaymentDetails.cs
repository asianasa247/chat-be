using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblAdvancePaymentDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FileStr",
            table: "RequestEquipments",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "AdvancePaymentDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                AdvancePaymentId = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdvancePaymentDetails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AdvancePaymentDetails");

        migrationBuilder.DropColumn(
            name: "FileStr",
            table: "RequestEquipments");
    }
}
