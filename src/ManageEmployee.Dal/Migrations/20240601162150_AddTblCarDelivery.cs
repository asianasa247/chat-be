using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblCarDelivery : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CarDeliveries",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TableId = table.Column<int>(type: "int", nullable: false),
                TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LicensePlates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Driver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneMumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FileLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarDeliveries", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CarDeliveries");
    }
}
