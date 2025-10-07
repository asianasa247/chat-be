using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblProcedureChangeShift : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "QuantityOriginal",
            table: "GoodRoomTypes");

        migrationBuilder.CreateTable(
            name: "ProcedureChangeShifts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProcedureNumber = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                FromUserId = table.Column<int>(type: "int", nullable: true),
                ToUserId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                FromAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ToAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsFinish = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcedureChangeShifts", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProcedureChangeShifts");

        migrationBuilder.AddColumn<int>(
            name: "QuantityOriginal",
            table: "GoodRoomTypes",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
