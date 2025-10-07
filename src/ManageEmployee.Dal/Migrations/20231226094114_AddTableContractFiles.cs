using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableContractFiles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DepartmentId",
            table: "ContractTypes");

        migrationBuilder.DropColumn(
            name: "LinkFile",
            table: "ContractTypes");

        migrationBuilder.DropColumn(
            name: "Type",
            table: "ContractTypes");

        migrationBuilder.CreateTable(
            name: "ContractFiles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                ContractTypeId = table.Column<int>(type: "int", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                LinkFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ContractFiles", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ContractFiles");

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

        migrationBuilder.AddColumn<string>(
            name: "Type",
            table: "ContractTypes",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
