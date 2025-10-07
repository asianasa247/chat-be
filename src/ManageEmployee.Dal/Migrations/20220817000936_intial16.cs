using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class intial16 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserTaskCheckLists",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserTaskId = table.Column<int>(type: "int", nullable: true),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FileLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<bool>(type: "bit", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTaskCheckLists", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserTaskComments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserTaskId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ParentId = table.Column<int>(type: "int", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                FileLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTaskComments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserTaskPins",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserTaskId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                OrderNo = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTaskPins", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserTaskRoleDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserTaskRoleId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                UserTaskId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTaskRoleDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserTasks",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ViewAll = table.Column<bool>(type: "bit", nullable: true),
                ParentId = table.Column<int>(type: "int", nullable: true),
                Status = table.Column<int>(type: "int", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                FileLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Viewer = table.Column<int>(type: "int", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTasks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserTaskTrackings",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserTaskId = table.Column<int>(type: "int", nullable: true),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserCreated = table.Column<int>(type: "int", nullable: true),
                UserUpdated = table.Column<int>(type: "int", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ActualHours = table.Column<double>(type: "float", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTaskTrackings", x => x.id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserTaskCheckLists");

        migrationBuilder.DropTable(
            name: "UserTaskComments");

        migrationBuilder.DropTable(
            name: "UserTaskPins");

        migrationBuilder.DropTable(
            name: "UserTaskRoleDetails");

        migrationBuilder.DropTable(
            name: "UserTasks");

        migrationBuilder.DropTable(
            name: "UserTaskTrackings");
    }
}
