using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddNewColumnInConfigAriseDocumentBehaviourTableMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "FocusFuntionalKeys",
            table: "ConfigAriseDocumentBehaviour",
            newName: "FocusFunctions");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "FocusFunctions",
            table: "ConfigAriseDocumentBehaviour",
            newName: "FocusFuntionalKeys");
    }
}
