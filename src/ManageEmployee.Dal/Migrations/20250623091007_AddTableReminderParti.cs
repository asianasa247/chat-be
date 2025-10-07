using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableReminderParti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReminderParticipant_Reminders_ReminderId",
                table: "ReminderParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReminderParticipant",
                table: "ReminderParticipant");

            migrationBuilder.RenameTable(
                name: "ReminderParticipant",
                newName: "ReminderParticipants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReminderParticipants",
                table: "ReminderParticipants",
                columns: new[] { "ReminderId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReminderParticipants_UserId",
                table: "ReminderParticipants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReminderParticipants_Reminders_ReminderId",
                table: "ReminderParticipants",
                column: "ReminderId",
                principalTable: "Reminders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReminderParticipants_Users_UserId",
                table: "ReminderParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReminderParticipants_Reminders_ReminderId",
                table: "ReminderParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ReminderParticipants_Users_UserId",
                table: "ReminderParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReminderParticipants",
                table: "ReminderParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ReminderParticipants_UserId",
                table: "ReminderParticipants");

            migrationBuilder.RenameTable(
                name: "ReminderParticipants",
                newName: "ReminderParticipant");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReminderParticipant",
                table: "ReminderParticipant",
                columns: new[] { "ReminderId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReminderParticipant_Reminders_ReminderId",
                table: "ReminderParticipant",
                column: "ReminderId",
                principalTable: "Reminders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
