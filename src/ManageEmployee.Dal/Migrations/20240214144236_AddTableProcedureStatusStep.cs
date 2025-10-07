using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableProcedureStatusStep : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "P_ProcedureStatusId",
            table: "P_ProcedureStatus");

        migrationBuilder.RenameColumn(
            name: "P_StatusName",
            table: "P_ProcedureStatus",
            newName: "Name");

        migrationBuilder.CreateTable(
            name: "P_ProcedureStatusSteps",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                P_ProcedureId = table.Column<int>(type: "int", nullable: false),
                ProcedureStatusIdFrom = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusIdTo = table.Column<int>(type: "int", nullable: true),
                IsFinish = table.Column<bool>(type: "bit", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_P_ProcedureStatusSteps", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProcedureRequestOvertimes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProcedureNumber = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                IsFinish = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcedureRequestOvertimes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Shifts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SymbolId = table.Column<int>(type: "int", nullable: false),
                TimeIn = table.Column<TimeSpan>(type: "time", nullable: false),
                TimeOut = table.Column<TimeSpan>(type: "time", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Shifts", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "P_ProcedureStatusSteps");

        migrationBuilder.DropTable(
            name: "ProcedureRequestOvertimes");

        migrationBuilder.DropTable(
            name: "Shifts");

        migrationBuilder.RenameColumn(
            name: "Name",
            table: "P_ProcedureStatus",
            newName: "P_StatusName");

        migrationBuilder.AddColumn<int>(
            name: "P_ProcedureStatusId",
            table: "P_ProcedureStatus",
            type: "int",
            nullable: true);
    }
}
