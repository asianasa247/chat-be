using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class ChangeParamUserCodeReceivedToUserIdReceived : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserCodeReceived",
            table: "BillTrackings");

        migrationBuilder.AddColumn<int>(
            name: "UserIdReceived",
            table: "BillTrackings",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserIdReceived",
            table: "BillTrackings");

        migrationBuilder.AddColumn<string>(
            name: "UserCodeReceived",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
