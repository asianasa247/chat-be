using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableFaceAcess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaceAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PhotoFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RepeatType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledPosts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaceAccesses");

            migrationBuilder.DropTable(
                name: "ScheduledPosts");
        }
    }
}
