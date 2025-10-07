using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial60 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "Point",
            table: "UserTasks",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<string>(
            name: "TypeWorkId",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "TypeWorkName",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<bool>(
            name: "isProject",
            table: "UserTasks",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "Prints",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Height = table.Column<int>(type: "int", nullable: false),
                Width = table.Column<int>(type: "int", nullable: false),
                Size = table.Column<int>(type: "int", nullable: false),
                TypePrint = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Prints", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TypeWorks",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Point = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TypeWorks", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Prints");

        migrationBuilder.DropTable(
            name: "TypeWorks");

        migrationBuilder.DropColumn(
            name: "Point",
            table: "UserTasks");

        migrationBuilder.DropColumn(
            name: "TypeWorkId",
            table: "UserTasks");

        migrationBuilder.DropColumn(
            name: "TypeWorkName",
            table: "UserTasks");

        migrationBuilder.DropColumn(
            name: "isProject",
            table: "UserTasks");
    }
}
