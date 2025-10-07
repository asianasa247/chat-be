using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class zalouser_24022025 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSync",
                table: "ZaloAppConfigs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ZaloUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserIdByApp = table.Column<long>(type: "bigint", nullable: false),
                    UserExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSensitive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserLastInteractionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserIsFollower = table.Column<bool>(type: "bit", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatars = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TagsAndNotesInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SharedInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaloUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZaloUsers");

            migrationBuilder.DropColumn(
                name: "LastSync",
                table: "ZaloAppConfigs");
        }
    }
}
