using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial106 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsNotAllowDelete",
            table: "UserRoles",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "Type",
            table: "Customers",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "BillHistoryCollections",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                StatusUserId = table.Column<int>(type: "int", nullable: false),
                StatusAccountantId = table.Column<int>(type: "int", nullable: false),
                Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BillHistoryCollections", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BillHistoryCollectionStatus",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BillHistoryCollectionStatus", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "BillHistoryCollections");

        migrationBuilder.DropTable(
            name: "BillHistoryCollectionStatus");

        migrationBuilder.DropColumn(
            name: "IsNotAllowDelete",
            table: "UserRoles");

        migrationBuilder.DropColumn(
            name: "Type",
            table: "Customers");
    }
}
