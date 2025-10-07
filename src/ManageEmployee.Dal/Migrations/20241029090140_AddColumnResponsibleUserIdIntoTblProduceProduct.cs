using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddColumnResponsibleUserIdIntoTblProduceProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExportProduct",
                table: "ProduceProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsImportProduct",
                table: "ProduceProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleUserId",
                table: "ProduceProducts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleUserId",
                table: "ManufactureOrders",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExportProduct",
                table: "ProduceProducts");

            migrationBuilder.DropColumn(
                name: "IsImportProduct",
                table: "ProduceProducts");

            migrationBuilder.DropColumn(
                name: "ResponsibleUserId",
                table: "ProduceProducts");

            migrationBuilder.DropColumn(
                name: "ResponsibleUserId",
                table: "ManufactureOrders");
        }
    }
}
