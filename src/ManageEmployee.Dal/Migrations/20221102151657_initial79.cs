using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial79 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreateAt",
            table: "Goods",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateApplicable",
            table: "Goods",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "UserCreated",
            table: "Goods",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreateAt",
            table: "Goods");

        migrationBuilder.DropColumn(
            name: "DateApplicable",
            table: "Goods");

        migrationBuilder.DropColumn(
            name: "UserCreated",
            table: "Goods");
    }
}
