using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTableWarningNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<double>(
            name: "VehicleLoad",
            table: "Cars",
            type: "float",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "VehicleBadgeNumber",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "TyreNumber",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Owner",
            table: "Cars",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "KmTestCone",
            table: "Cars",
            type: "float",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "KmTestBrake",
            table: "Cars",
            type: "float",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "KmChangeOil",
            table: "Cars",
            type: "float",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Cavet",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccumulatorNumber",
            table: "Cars",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "WarningNotifications",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                WarningId = table.Column<int>(type: "int", nullable: false),
                WarningTableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WarningNotifications", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "WarningNotifications");

        migrationBuilder.AlterColumn<string>(
            name: "VehicleLoad",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "VehicleBadgeNumber",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "TyreNumber",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Owner",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "KmTestCone",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "KmTestBrake",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "KmChangeOil",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Cavet",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccumulatorNumber",
            table: "Cars",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);
    }
}
