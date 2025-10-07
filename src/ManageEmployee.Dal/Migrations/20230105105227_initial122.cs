using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial122 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsManager",
            table: "PositionDetails",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.InsertData(
            table: "SalarySocials",
            columns: new[] { "Id", "AccountCredit", "AccountDebit", "Code", "DetailCredit1", "DetailCredit2", "DetailDebit1", "DetailDebit2", "Name", "Order", "ValueCompany", "ValueUser" },
            values: new object[] { 5, "", "", "LUONGQUANLY", "", "", "", "", "Lương quản lý", 0, 0.0, 0.0 });

        migrationBuilder.InsertData(
            table: "SalarySocials",
            columns: new[] { "Id", "AccountCredit", "AccountDebit", "Code", "DetailCredit1", "DetailCredit2", "DetailDebit1", "DetailDebit2", "Name", "Order", "ValueCompany", "ValueUser" },
            values: new object[] { 6, "", "", "LUONGNHANVIEN", "", "", "", "", "Lương nhân viên", 0, 0.0, 0.0 });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "SalarySocials",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "SalarySocials",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DropColumn(
            name: "IsManager",
            table: "PositionDetails");
    }
}
