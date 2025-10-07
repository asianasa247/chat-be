using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial29 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                Note = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                NumberItem = table.Column<int>(type: "int", nullable: true),
                isPublish = table.Column<bool>(type: "bit", nullable: false),
                Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                CodeParent = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                NameEnglish = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                NameKorea = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                isEnableDelete = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Introduces",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Type = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                IframeYoutube = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                IntroduceType = table.Column<int>(type: "int", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Introduces", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Categories");

        migrationBuilder.DropTable(
            name: "Introduces");
    }
}
