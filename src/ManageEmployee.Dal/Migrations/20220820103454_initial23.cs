using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial23 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Currency",
            table: "ChartOfAccounts",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.CreateTable(
            name: "LookupValues",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LookupValues", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LookupValues");

        migrationBuilder.AlterColumn<string>(
            name: "Currency",
            table: "ChartOfAccounts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
