using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddPropertyNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentEnglish",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentKorean",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageEnglish",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageKorean",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortContentEnglish",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortContentKorean",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEnglish",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleKorean",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ConfigurationViews",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "[\n                        { label: 'Tiền mặt', value: 'TM' },\n                                { label: 'Công nợ', value: 'CN' },\n                                { label: 'Ngân hàng', value: 'NH' },\n                    ]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentEnglish",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ContentKorean",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ImageEnglish",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ImageKorean",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ShortContentEnglish",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ShortContentKorean",
                table: "News");

            migrationBuilder.DropColumn(
                name: "TitleEnglish",
                table: "News");

            migrationBuilder.DropColumn(
                name: "TitleKorean",
                table: "News");

            migrationBuilder.UpdateData(
                table: "ConfigurationViews",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "[\r\n                        { label: 'Tiền mặt', value: 'TM' },\r\n                                { label: 'Công nợ', value: 'CN' },\r\n                                { label: 'Ngân hàng', value: 'NH' },\r\n                    ]");
        }
    }
}
