using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class Addconfigarisebehaviour : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ConfigAriseBehaviour",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CodeData = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Index = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConfigAriseBehaviour", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ConfigAriseDocumentBehaviour",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                AriseBehaviourId = table.Column<int>(type: "int", nullable: false),
                DocumentId = table.Column<int>(type: "int", nullable: false),
                NokeepDataChartOfAccount = table.Column<bool>(type: "bit", nullable: false),
                NokeepDataBill = table.Column<bool>(type: "bit", nullable: false),
                NokeepDataTax = table.Column<bool>(type: "bit", nullable: false),
                FocusLedger = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ConfigAriseDocumentBehaviour", x => x.Id);
                table.ForeignKey(
                    name: "FK_ConfigAriseDocumentBehaviour_ConfigAriseBehaviour_AriseBehaviourId",
                    column: x => x.AriseBehaviourId,
                    principalTable: "ConfigAriseBehaviour",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ConfigAriseDocumentBehaviour_AriseBehaviourId",
            table: "ConfigAriseDocumentBehaviour",
            column: "AriseBehaviourId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ConfigAriseDocumentBehaviour");

        migrationBuilder.DropTable(
            name: "ConfigAriseBehaviour");
    }
}
