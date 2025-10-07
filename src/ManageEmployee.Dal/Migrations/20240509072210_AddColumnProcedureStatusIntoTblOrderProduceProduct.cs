using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnProcedureStatusIntoTblOrderProduceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ProcedureNumber",
            table: "OrderProduceProducts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ProcedureStatusId",
            table: "OrderProduceProducts",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ProcedureStatusName",
            table: "OrderProduceProducts",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ProcedureNumber",
            table: "OrderProduceProducts");

        migrationBuilder.DropColumn(
            name: "ProcedureStatusId",
            table: "OrderProduceProducts");

        migrationBuilder.DropColumn(
            name: "ProcedureStatusName",
            table: "OrderProduceProducts");
    }
}
