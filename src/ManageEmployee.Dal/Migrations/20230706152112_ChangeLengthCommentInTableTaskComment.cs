using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class ChangeLengthCommentInTableTaskComment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Comment",
            table: "UserTaskComments",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Comment",
            table: "UserTaskComments",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
