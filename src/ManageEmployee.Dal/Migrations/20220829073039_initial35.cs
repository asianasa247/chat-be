using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial35 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<short>(
            name: "Gender",
            table: "Customers",
            type: "smallint",
            nullable: false,
            defaultValue: (short)0,
            oldClrType: typeof(bool),
            oldType: "bit",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "Decide",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Type = table.Column<int>(type: "int", nullable: true),
                EmployeesId = table.Column<int>(type: "int", nullable: true),
                EmployeesName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                DecideTypeId = table.Column<int>(type: "int", nullable: true),
                DecideTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                FileUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Decide", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "HistoryAchievements",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: true),
                Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: true),
                UserUpdated = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HistoryAchievements", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Decide");

        migrationBuilder.DropTable(
            name: "HistoryAchievements");

        migrationBuilder.AlterColumn<bool>(
            name: "Gender",
            table: "Customers",
            type: "bit",
            nullable: true,
            oldClrType: typeof(short),
            oldType: "smallint");
    }
}
