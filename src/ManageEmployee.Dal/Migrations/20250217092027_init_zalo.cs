using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class init_zalo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZaloAppConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OauthCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaloAppConfigs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZaloAppConfigs");
        }
    }
}
