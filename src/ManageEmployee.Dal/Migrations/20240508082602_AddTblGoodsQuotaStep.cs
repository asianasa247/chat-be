using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblGoodsQuotaStep : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "GoodsQuotaStepId",
            table: "GoodsQuotas",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "GoodsQuotaStepId",
            table: "GoodsQuotaRecipes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "GoodsQuotaSteps",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsQuotaSteps", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodsQuotaSteps");

        migrationBuilder.DropColumn(
            name: "GoodsQuotaStepId",
            table: "GoodsQuotas");

        migrationBuilder.DropColumn(
            name: "GoodsQuotaStepId",
            table: "GoodsQuotaRecipes");
    }
}
