using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class RemoveYearInAdditionWebGoods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionWebGoodsSelections");

            migrationBuilder.CreateTable(
                name: "AdditionWebGoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionWebId = table.Column<int>(type: "int", nullable: false),
                    GoodId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionWebGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionWebGoods_AdditionWebs_AdditionWebId",
                        column: x => x.AdditionWebId,
                        principalTable: "AdditionWebs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionWebGoods_AdditionWebId",
                table: "AdditionWebGoods",
                column: "AdditionWebId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionWebGoods");

            migrationBuilder.CreateTable(
                name: "AdditionWebGoodsSelections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionWebId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GoodId = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionWebGoodsSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionWebGoodsSelections_AdditionWebs_AdditionWebId",
                        column: x => x.AdditionWebId,
                        principalTable: "AdditionWebs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionWebGoodsSelections_AdditionWebId",
                table: "AdditionWebGoodsSelections",
                column: "AdditionWebId");
        }
    }
}
