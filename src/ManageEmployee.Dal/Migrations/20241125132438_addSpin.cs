using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class addSpin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorySpins",
                columns: table => new
                {
                    SpinId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSettingsSpin = table.Column<int>(type: "int", nullable: false),
                    PrizeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Warehouse = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    WarehouseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DetailName1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detail2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DetailName2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    WinTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorySpins", x => x.SpinId);
                });

            migrationBuilder.CreateTable(
                name: "Prizes",
                columns: table => new
                {
                    PrizeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdSettingsSpin = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrdinalSpin = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prizes", x => x.PrizeId);
                });

            migrationBuilder.CreateTable(
                name: "SettingsSpins",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCustomerClassification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeStartSpin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimePerSpin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AwarDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsSpins", x => x.SettingId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorySpins");

            migrationBuilder.DropTable(
                name: "Prizes");

            migrationBuilder.DropTable(
                name: "SettingsSpins");
        }
    }
}
