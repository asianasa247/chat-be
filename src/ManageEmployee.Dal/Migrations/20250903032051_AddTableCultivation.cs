using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTableCultivation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) PlantingTypes
            migrationBuilder.CreateTable(
                name: "PlantingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantingTypes", x => x.Id);
                });

            // 2) PlantingRegions
            migrationBuilder.CreateTable(
                name: "PlantingRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Manager = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IssuerUnitCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantingRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantingRegions_PlantingTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PlantingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict // NO ACTION/RESTRICT
                    );
                });

            // 3) PlantingBeds
            migrationBuilder.CreateTable(
                name: "PlantingBeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    StartYear = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantingBeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantingBeds_PlantingRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "PlantingRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade // cascade Region -> Bed
                    );
                    table.ForeignKey(
                        name: "FK_PlantingBeds_PlantingTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PlantingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict // NO ACTION/RESTRICT
                    );
                });

            // Indexes / Unique
            migrationBuilder.CreateIndex(
                name: "IX_PlantingTypes_Category_Code",
                table: "PlantingTypes",
                columns: new[] { "Category", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantingRegions_CountryId_Code",
                table: "PlantingRegions",
                columns: new[] { "CountryId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantingRegions_TypeId",
                table: "PlantingRegions",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantingBeds_RegionId_Code",
                table: "PlantingBeds",
                columns: new[] { "RegionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantingBeds_TypeId",
                table: "PlantingBeds",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PlantingBeds");
            migrationBuilder.DropTable(name: "PlantingRegions");
            migrationBuilder.DropTable(name: "PlantingTypes");
        }
    }
}
