using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial116 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssetUser",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssets",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAsset242",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssetUser",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);
    }
}
