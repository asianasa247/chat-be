using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AlterLengthNameFixasset : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TotalInvoice",
            table: "InvoiceDeclarations");

        migrationBuilder.DropColumn(
            name: "TaxVATPercent",
            table: "Goods");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ManagerName",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Wards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Wards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Wards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "TypeWorkName",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserTaskCheckLists",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "FileLink",
            table: "UserTaskCheckLists",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Username",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "UserRoles",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "UserRoles",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "TaxRates",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "NameContact",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "IdentityCode",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Address",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Symbols",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Symbols",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Symbols",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Surcharges",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Surcharges",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Surcharges",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Subsidizes",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Subsidizes",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Status",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "Status",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Social",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ShortContent",
            table: "Social",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Image",
            table: "Social",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Social",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Sliders",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Img",
            table: "Sliders",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailDebit2",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailDebit1",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailCredit2",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailCredit1",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "AccountDebit",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "AccountCredit",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "PositionName",
            table: "SalaryLevel",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "SalaryLevel",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "SalaryLevel",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ZipCode",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Prints",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Positions",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Positions",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "PositionDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_SalaryAdvance",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_SalaryAdvance",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "P_StatusName",
            table: "P_ProcedureStatus",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Procedure",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "P_Procedure",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Leave",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Leave",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Kpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Kpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Tell",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ShippingAddress",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Order",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "MenuKpis",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "MenuKpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "MenuKpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Majors",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Majors",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Majors",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Value",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Scope",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Ledgers",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "LedgerInternals",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ClassName",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1Parent",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Detail1Parent",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Detail1",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Account",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssets",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAssets",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAssets",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAsset242",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAsset242",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Employees",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DocumentTypes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "DocumentTypes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Districts",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Districts",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Districts",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Descriptions",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecond",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirst",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecond",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirst",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Departments",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Departments",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Degrees",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Degrees",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DecisionType",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "DecisionType",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Customers",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Customers",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Customers",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "StockUnit",
            table: "CustomerQuote_Detail",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "CustomerQuote",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "CustomerClassifications",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "CustomerClassifications",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "CustomerClassifications",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ContractTypes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ContractTypes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "MST",
            table: "Companies",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ChartOfAcountTests",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ChartOfAcountTests",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ChartOfAccounts",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ChartOfAccounts",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ChartOfAccountGroups",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ChartOfAccountGroups",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CodeChartOfAccountGroup",
            table: "ChartOfAccountGroupLinks",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CodeChartOfAccount",
            table: "ChartOfAccountGroupLinks",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Branchs",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "UserCode",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "TranType",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Status",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "UserCode",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "TypePay",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Status",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "CustomerName",
            table: "Bills",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);

        migrationBuilder.AlterColumn<string>(
            name: "DiscountType",
            table: "BillDetails",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Allowances",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Allowances",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);

        migrationBuilder.AlterColumn<string>(
            name: "Account",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ManagerName",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Wards",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Wards",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Wards",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "TypeWorkName",
            table: "UserTasks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "UserTaskCheckLists",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FileLink",
            table: "UserTaskCheckLists",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Username",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "UserRoles",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "UserRoles",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "TypeWorks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "TaxRates",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "TaxRates",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "TaxRates",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Phone",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "NameContact",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "IdentityCode",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Address",
            table: "Targets",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Symbols",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Symbols",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Symbols",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Surcharges",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Surcharges",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Surcharges",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Subsidizes",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Subsidizes",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Status",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "Status",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "Social",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ShortContent",
            table: "Social",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Image",
            table: "Social",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Social",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Sliders",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Img",
            table: "Sliders",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailDebit2",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailDebit1",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailCredit2",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailCredit1",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccountDebit",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccountCredit",
            table: "SalarySocials",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "PositionName",
            table: "SalaryLevel",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "SalaryLevel",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "SalaryLevel",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ZipCode",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Provinces",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Prints",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Positions",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Positions",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "PositionDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_SalaryAdvance",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_SalaryAdvance",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "P_StatusName",
            table: "P_ProcedureStatus",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Procedure",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "P_Procedure",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Leave",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Leave",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Kpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Kpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ProcedureNumber",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Tell",
            table: "Order",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ShippingAddress",
            table: "Order",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "FullName",
            table: "Order",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "MenuKpis",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "MenuKpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "MenuKpis",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "Majors",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Majors",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Majors",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Value",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Scope",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "LookupValues",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Ledgers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "LedgerInternals",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ClassName",
            table: "IsoftHistory",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "TotalInvoice",
            table: "InvoiceDeclarations",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "TaxVATPercent",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1Parent",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DetailName1",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Detail1Parent",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Detail1",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Account",
            table: "GoodDetails",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAssets",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAssets",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "FixedAsset242",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecondName",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirstName",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCodeName",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecondName",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirstName",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCodeName",
            table: "FixedAsset242",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Employees",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DocumentTypes",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "DocumentTypes",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "Districts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Districts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Districts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Descriptions",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeSecond",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitDetailCodeFirst",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DebitCode",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeSecond",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditDetailCodeFirst",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CreditCode",
            table: "Descriptions",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Departments",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Departments",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Degrees",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Degrees",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "DecisionType",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "DecisionType",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Customers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Customers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Customers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "StockUnit",
            table: "CustomerQuote_Detail",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "CustomerQuote",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "CustomerClassifications",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "CustomerClassifications",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Color",
            table: "CustomerClassifications",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ContractTypes",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ContractTypes",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "MST",
            table: "Companies",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ChartOfAcountTests",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ChartOfAcountTests",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ChartOfAccounts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ChartOfAccounts",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "ChartOfAccountGroups",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "ChartOfAccountGroups",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CodeChartOfAccountGroup",
            table: "ChartOfAccountGroupLinks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CodeChartOfAccount",
            table: "ChartOfAccountGroupLinks",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Branchs",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCode",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "TranType",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Status",
            table: "BillTrackings",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCode",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "TypePay",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Status",
            table: "Bills",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "CustomerName",
            table: "Bills",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "DiscountType",
            table: "BillDetails",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Allowances",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "Allowances",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccountName",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Account",
            table: "AccountPays",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);
    }
}
