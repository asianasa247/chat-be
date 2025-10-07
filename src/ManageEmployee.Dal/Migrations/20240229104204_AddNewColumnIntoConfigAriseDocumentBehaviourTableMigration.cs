using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddNewColumnIntoConfigAriseDocumentBehaviourTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FocusFuntionalKeys",
            table: "ConfigAriseDocumentBehaviour",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FocusFuntionalKeys",
            table: "ConfigAriseDocumentBehaviour");
    }
}
