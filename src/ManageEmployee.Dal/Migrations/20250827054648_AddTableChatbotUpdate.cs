using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableChatbotUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatBotScheduledMessages");

            migrationBuilder.DropTable(
                name: "ChatBotZalos");

            migrationBuilder.DropTable(
                name: "ChatBotTopics");

            migrationBuilder.CreateTable(
                name: "ChatboxAIQAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatboxAIQAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatboxAIQAs_ChatboxAITopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "ChatboxAITopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatboxAIScheduledMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SendTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatboxAIScheduledMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatboxAIScheduledMessages_ChatboxAITopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "ChatboxAITopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatboxAIQAs_TopicId",
                table: "ChatboxAIQAs",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatboxAIScheduledMessages_TopicId",
                table: "ChatboxAIScheduledMessages",
                column: "TopicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatboxAIQAs");

            migrationBuilder.DropTable(
                name: "ChatboxAIScheduledMessages");

            migrationBuilder.CreateTable(
                name: "ChatBotTopics",
                columns: table => new
                {
                    IdTopic = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatBotTopics", x => x.IdTopic);
                });

            migrationBuilder.CreateTable(
                name: "ChatBotScheduledMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTopic = table.Column<int>(type: "int", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatBotScheduledMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatBotScheduledMessages_ChatBotTopics_IdTopic",
                        column: x => x.IdTopic,
                        principalTable: "ChatBotTopics",
                        principalColumn: "IdTopic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatBotZalos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTopic = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatBotZalos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatBotZalos_ChatBotTopics_IdTopic",
                        column: x => x.IdTopic,
                        principalTable: "ChatBotTopics",
                        principalColumn: "IdTopic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatBotScheduledMessages_IdTopic",
                table: "ChatBotScheduledMessages",
                column: "IdTopic");

            migrationBuilder.CreateIndex(
                name: "IX_ChatBotZalos_IdTopic",
                table: "ChatBotZalos",
                column: "IdTopic");
        }
    }
}
