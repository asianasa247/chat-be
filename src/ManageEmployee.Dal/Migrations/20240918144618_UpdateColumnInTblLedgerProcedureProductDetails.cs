using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInTblLedgerProcedureProductDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_LedgerProduceProductDetails",
            table: "LedgerProduceProductDetails");

        migrationBuilder.RenameTable(
            name: "LedgerProduceProductDetails",
            newName: "LedgerProcedureProductDetails");

        migrationBuilder.RenameColumn(
            name: "LedgerProduceProductId",
            table: "LedgerProcedureProductDetails",
            newName: "LedgerProcedureProductId");

        migrationBuilder.AlterColumn<int>(
            name: "OrderProduceProductId",
            table: "PlanningProduceProductDetails",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddPrimaryKey(
            name: "PK_LedgerProcedureProductDetails",
            table: "LedgerProcedureProductDetails",
            column: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_LedgerProcedureProductDetails",
            table: "LedgerProcedureProductDetails");

        migrationBuilder.RenameTable(
            name: "LedgerProcedureProductDetails",
            newName: "LedgerProduceProductDetails");

        migrationBuilder.RenameColumn(
            name: "LedgerProcedureProductId",
            table: "LedgerProduceProductDetails",
            newName: "LedgerProduceProductId");

        migrationBuilder.AlterColumn<int>(
            name: "OrderProduceProductId",
            table: "PlanningProduceProductDetails",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_LedgerProduceProductDetails",
            table: "LedgerProduceProductDetails",
            column: "Id");
    }
}
