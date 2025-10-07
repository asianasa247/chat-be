using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class inital92 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "P_Status");

        migrationBuilder.RenameColumn(
            name: "P_StatusId",
            table: "P_ProcedureStatus",
            newName: "Type");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Type",
            table: "P_ProcedureStatus",
            newName: "P_StatusId");

        migrationBuilder.CreateTable(
            name: "P_Status",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Type = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_Status", x => x.Id);
            });
    }
}
