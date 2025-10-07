using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RenameColumnPhoneNumberInTblCarDelivery : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PhoneMumber",
            table: "CarDeliveries",
            newName: "PhoneNumber");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PhoneNumber",
            table: "CarDeliveries",
            newName: "PhoneMumber");
    }
}
