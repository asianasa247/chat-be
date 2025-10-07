using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial114 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Discriminator",
            table: "FixedAsset242",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "SendMails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true),
                Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreateSend = table.Column<DateTime>(type: "datetime2", nullable: true),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SendMails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SendMails");

        migrationBuilder.DropColumn(
            name: "Discriminator",
            table: "FixedAsset242");
    }
}
