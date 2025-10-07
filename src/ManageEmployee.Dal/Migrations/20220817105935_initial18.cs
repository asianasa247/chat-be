using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial18 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsProtected",
            table: "ChartOfAccounts");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "UserTaskTrackings",
            newName: "Id");

        migrationBuilder.AlterColumn<string>(
            name: "Username",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "SocialInsuranceCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ShareHolderCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Religion",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "PlaceOfPermanent",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Users",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "PersonalTaxCode",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Nation",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "LiteracyDetail",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Literacy",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "LicensePlates",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Language",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "IdentifyCreatedPlace",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Identify",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Facebook",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "EthnicGroup",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CertificateOther",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "BankAccount",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Bank",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Avatar",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Address",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<int>(
            name: "Protected",
            table: "ChartOfAccounts",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "Payers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                TaxCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                BankNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                BankName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                IdentityNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Product = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                PayerType = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Payers", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Payers");

        migrationBuilder.DropColumn(
            name: "Protected",
            table: "ChartOfAccounts");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "UserTaskTrackings",
            newName: "id");

        migrationBuilder.AlterColumn<string>(
            name: "Username",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "SocialInsuranceCode",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "ShareHolderCode",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Religion",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "PlaceOfPermanent",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20);

        migrationBuilder.AlterColumn<string>(
            name: "PersonalTaxCode",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Nation",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "LiteracyDetail",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Literacy",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "LicensePlates",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Language",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "IdentifyCreatedPlace",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Identify",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Facebook",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "EthnicGroup",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "CertificateOther",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "BankAccount",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Bank",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Avatar",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Address",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AddColumn<bool>(
            name: "IsProtected",
            table: "ChartOfAccounts",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }
}
