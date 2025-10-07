using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddSeedDataSalarySocial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "SalarySocials",
            columns: new[] { "Id", "AccountCredit", "AccountDebit", "Code", "DetailCredit1", "DetailCredit2", "DetailDebit1", "DetailDebit2", "Name", "Order", "ValueCompany", "ValueUser" },
            values: new object[,]
            {
                { 1, "", "", "KPCD", "", "", "", "", "Kinh phí công đoàn", 1, 2.0, 0.0 },
                { 2, "", "", "BHXH", "", "", "", "", "Bảo hiểm xã hội", 1, 17.0, 8.0 },
                { 3, "", "", "BHYT", "", "", "", "", "Bảo hiểm y tế", 1, 3.0, 1.5 },
                { 4, "", "", "BHTN", "", "", "", "", "Bảo hiểm thất nghiệp", 1, 1.0, 1.0 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "SalarySocials",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "SalarySocials",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "SalarySocials",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "SalarySocials",
            keyColumn: "Id",
            keyValue: 4);
    }
}
