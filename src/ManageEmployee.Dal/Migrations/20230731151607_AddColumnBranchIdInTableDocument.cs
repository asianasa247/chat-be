using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnBranchIdInTableDocument : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "BranchId",
            table: "DocumentType2",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "BranchId",
            table: "DocumentType1",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BranchId",
            table: "DocumentType2");

        migrationBuilder.DropColumn(
            name: "BranchId",
            table: "DocumentType1");
    }
}
