using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial47 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Certificate",
            table: "Relatives");

        migrationBuilder.RenameColumn(
            name: "Specialize",
            table: "Relatives",
            newName: "Degree");

        migrationBuilder.RenameColumn(
            name: "LiteracyDetail",
            table: "Relatives",
            newName: "CertificateOther");

        migrationBuilder.AddColumn<int>(
            name: "MajorId",
            table: "Relatives",
            type: "int",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MajorId",
            table: "Relatives");

        migrationBuilder.RenameColumn(
            name: "Degree",
            table: "Relatives",
            newName: "Specialize");

        migrationBuilder.RenameColumn(
            name: "CertificateOther",
            table: "Relatives",
            newName: "LiteracyDetail");

        migrationBuilder.AddColumn<string>(
            name: "Certificate",
            table: "Relatives",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }
}
