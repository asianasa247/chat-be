using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class Addmenuitemchatboxaitopic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO dbo.Menus
                        (Code, Name, CodeParent, Note, NameEN, NameKO, IsParent, [Order])
                        VALUES(N'CHATBOXAITOPIC', N'Chủ đề Chatbox AI', N'MARKETING', N'', 'Chatbox AI Topic', NULL, 0, 9);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
