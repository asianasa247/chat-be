using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblProcedureLogs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsSpecialOrder",
            table: "OrderProduceProducts",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "ProcedureLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProcedureId = table.Column<int>(type: "int", nullable: false),
                ProcedureCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcedureLogs", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name" },
            values: new object[] { 4, "SpecialOrder", "Đơn hàng đặc biệt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProcedureLogs");

        migrationBuilder.DeleteData(
            table: "ProcedureConditions",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DropColumn(
            name: "IsSpecialOrder",
            table: "OrderProduceProducts");
    }
}
