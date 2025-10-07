using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableChatSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatSupportRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSupportRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatSupportMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSupportMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSupportMessages_ChatSupportRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ChatSupportRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatSupportParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSupportParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSupportParticipants_ChatSupportRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ChatSupportRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatSupportMessages_RoomId_SentAt",
                table: "ChatSupportMessages",
                columns: new[] { "RoomId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatSupportParticipants_RoomId_UserId",
                table: "ChatSupportParticipants",
                columns: new[] { "RoomId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatSupportMessages");

            migrationBuilder.DropTable(
                name: "ChatSupportParticipants");

            migrationBuilder.DropTable(
                name: "ChatSupportRooms");
        }
    }
}
