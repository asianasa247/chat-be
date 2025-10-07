using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddAlePayAndNewVarForCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventCustomerId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlepayConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsSandbox = table.Column<bool>(type: "bit", nullable: false),
                    TokenKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ChecksumKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CustomMerchantId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AdditionWebId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlepayConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionWebId = table.Column<int>(type: "int", nullable: true),
                    OrderCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Installment = table.Column<bool>(type: "bit", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: true),
                    ReturnUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CancelUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BuyerName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BuyerEmail = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BuyerPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ProviderPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebhookPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentWebhookLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Raw = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentWebhookLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlepayConfigs");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PaymentWebhookLogs");

            migrationBuilder.DropColumn(
                name: "EventCustomerId",
                table: "Customers");
        }
    }
}
