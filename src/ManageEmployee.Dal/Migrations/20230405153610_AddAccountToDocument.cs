using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddAccountToDocument : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreditCodeFirst",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CreditCodeFirstName",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CreditCodeSecond",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CreditCodeSecondName",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DebitCodeFirst",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DebitCodeFirstName",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DebitCodeSecond",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DebitCodeSecondName",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreditCodeFirst",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "CreditCodeFirstName",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "CreditCodeSecond",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "CreditCodeSecondName",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "DebitCodeFirst",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "DebitCodeFirstName",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "DebitCodeSecond",
            table: "Documents");

        migrationBuilder.DropColumn(
            name: "DebitCodeSecondName",
            table: "Documents");
    }
}
