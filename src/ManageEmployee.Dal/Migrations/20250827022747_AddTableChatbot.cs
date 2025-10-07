using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableChatbot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatBotTopics",
                columns: table => new
                {
                    IdTopic = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatBotScheduledMessages");

            migrationBuilder.DropTable(
                name: "ChatBotZalos");

            migrationBuilder.DropTable(
                name: "ChatBotTopics");
        }
    }
}
