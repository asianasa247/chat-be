using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblProcedureCondition : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ProcedureConditionId",
            table: "P_ProcedureStatusSteps",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "ProcedureConditions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcedureConditions", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "ProcedureConditions",
            columns: new[] { "Id", "Code", "Name" },
            values: new object[] { 1, "PriceLower", "Kiểm tra đơn giá thấp hơn hiện tại" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProcedureConditions");

        migrationBuilder.DropColumn(
            name: "ProcedureConditionId",
            table: "P_ProcedureStatusSteps");
    }
}
