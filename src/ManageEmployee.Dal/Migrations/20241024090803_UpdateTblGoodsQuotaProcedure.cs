using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class UpdateTblGoodsQuotaProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GoodsQuotas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GoodsQuotaCode",
                table: "GoodsQuotas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "GoodsQuotas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NoteNotAccept",
                table: "GoodsQuotas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcedureNumber",
                table: "GoodsQuotas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcedureStatusId",
                table: "GoodsQuotas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcedureStatusName",
                table: "GoodsQuotas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "GoodsQuotas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserCreated",
                table: "GoodsQuotas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserUpdated",
                table: "GoodsQuotas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "GoodsQuotaCode",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "NoteNotAccept",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "ProcedureNumber",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "ProcedureStatusId",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "ProcedureStatusName",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "GoodsQuotas");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "GoodsQuotas");
        }
    }
}
