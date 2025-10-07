using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial6 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "TimeOut",
            table: "Symbols",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(TimeSpan),
            oldType: "time");

        migrationBuilder.AlterColumn<DateTime>(
            name: "TimeIn",
            table: "Symbols",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(TimeSpan),
            oldType: "time");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<TimeSpan>(
            name: "TimeOut",
            table: "Symbols",
            type: "time",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime2");

        migrationBuilder.AlterColumn<TimeSpan>(
            name: "TimeIn",
            table: "Symbols",
            type: "time",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime2");
    }
}
