using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial12 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CustomerContactHistories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                Contact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                NextTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                ExchangeContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                StatusId = table.Column<int>(type: "int", nullable: true),
                JobsId = table.Column<int>(type: "int", nullable: true),
                FileLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerContactHistories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                Gender = table.Column<bool>(type: "bit", nullable: true),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProvinceId = table.Column<int>(type: "int", nullable: true),
                DistrictId = table.Column<int>(type: "int", nullable: true),
                WardId = table.Column<int>(type: "int", nullable: true),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SendEmail = table.Column<bool>(type: "bit", nullable: true),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Facebook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentityCardNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentityCardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IdentityCardIssuePlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentityCardValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                IdentityCardProvinceId = table.Column<int>(type: "int", nullable: true),
                IdentityCardDistrictId = table.Column<int>(type: "int", nullable: true),
                IdentityCardWardId = table.Column<int>(type: "int", nullable: true),
                IdentityCardPlaceOfPermanent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdentityCardAddressInCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: true),
                UserUpdated = table.Column<int>(type: "int", nullable: true),
                Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CustomerClassficationId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CustomerTaxInformations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Bank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Accountant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PhoneOfAccountant = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerTaxInformations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DocumentType1",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                DocumentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TextSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DateText = table.Column<DateTime>(type: "datetime2", nullable: true),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Signer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReceiverId = table.Column<int>(type: "int", nullable: true),
                ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DocumentType1", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "DocumentType2",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DocumentId = table.Column<int>(type: "int", nullable: false),
                DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TextSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DateText = table.Column<DateTime>(type: "datetime2", nullable: true),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DraftarId = table.Column<int>(type: "int", nullable: true),
                DraftarName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SignerTextId = table.Column<int>(type: "int", nullable: true),
                SignerTextName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DocumentType2", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FixedAssets",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                HistoricalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                EndOfDepreciation = table.Column<DateTime>(type: "datetime2", nullable: true),
                LiquidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                TotalMonth = table.Column<int>(type: "int", nullable: true),
                DepreciationOfOneDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                AccruedExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                TotalDayDepreciationOfThisPeriod = table.Column<int>(type: "int", nullable: true),
                DepreciationOfThisPeriod = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                CarryingAmountOfLiquidationAsset = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                CarryingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                DepartmentManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Use = table.Column<short>(type: "smallint", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FixedAssets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Jobs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CompanyId = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<bool>(type: "bit", nullable: false),
                Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Jobs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Status",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CompanyId = table.Column<int>(type: "int", nullable: false),
                StatusDetect = table.Column<bool>(type: "bit", nullable: false),
                Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Status", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CustomerContactHistories");

        migrationBuilder.DropTable(
            name: "Customers");

        migrationBuilder.DropTable(
            name: "CustomerTaxInformations");

        migrationBuilder.DropTable(
            name: "DocumentType1");

        migrationBuilder.DropTable(
            name: "DocumentType2");

        migrationBuilder.DropTable(
            name: "FixedAssets");

        migrationBuilder.DropTable(
            name: "Jobs");

        migrationBuilder.DropTable(
            name: "Status");
    }
}
