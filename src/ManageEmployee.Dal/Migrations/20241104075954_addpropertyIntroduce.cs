using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class addpropertyIntroduce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentEnglish",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentKorean",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IframeYoutubeEnglish",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IframeYoutubeKorean",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortContentEnglish",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortContentKorean",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEnglish",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleKorean",
                table: "Introduces",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentEnglish",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "ContentKorean",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "IframeYoutubeEnglish",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "IframeYoutubeKorean",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "ShortContentEnglish",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "ShortContentKorean",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "TitleEnglish",
                table: "Introduces");

            migrationBuilder.DropColumn(
                name: "TitleKorean",
                table: "Introduces");
        }
    }
}
