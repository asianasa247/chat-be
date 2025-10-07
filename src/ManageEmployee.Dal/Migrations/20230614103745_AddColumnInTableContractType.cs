using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnInTableContractType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ContractDepartments");

        migrationBuilder.AddColumn<int>(
            name: "DepartmentId",
            table: "ContractTypes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "LinkFile",
            table: "ContractTypes",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DepartmentId",
            table: "ContractTypes");

        migrationBuilder.DropColumn(
            name: "LinkFile",
            table: "ContractTypes");

        migrationBuilder.CreateTable(
            name: "ContractDepartments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ContractTypeId = table.Column<int>(type: "int", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: false),
                LinkFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContractDepartments", x => x.Id);
            });
    }
}
