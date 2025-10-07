using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial68 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TypePrint",
            table: "Prints");

        migrationBuilder.AlterColumn<int>(
            name: "Width",
            table: "Prints",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "Size",
            table: "Prints",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<int>(
            name: "Height",
            table: "Prints",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddColumn<double>(
            name: "MarginBottom",
            table: "Prints",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "MarginLeft",
            table: "Prints",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "MarginRight",
            table: "Prints",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "MarginTop",
            table: "Prints",
            type: "float",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Name",
            table: "Prints",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailName2",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Detail2",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Detail1",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Account",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.CreateTable(
            name: "PagePrints",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Height = table.Column<int>(type: "int", nullable: false),
                Width = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PagePrints", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PagePrints");

        migrationBuilder.DropColumn(
            name: "MarginBottom",
            table: "Prints");

        migrationBuilder.DropColumn(
            name: "MarginLeft",
            table: "Prints");

        migrationBuilder.DropColumn(
            name: "MarginRight",
            table: "Prints");

        migrationBuilder.DropColumn(
            name: "MarginTop",
            table: "Prints");

        migrationBuilder.DropColumn(
            name: "Name",
            table: "Prints");

        migrationBuilder.AlterColumn<int>(
            name: "Width",
            table: "Prints",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Size",
            table: "Prints",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Height",
            table: "Prints",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TypePrint",
            table: "Prints",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName2",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Detail2",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Detail1",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Account",
            table: "AccountPays",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);
    }
}
