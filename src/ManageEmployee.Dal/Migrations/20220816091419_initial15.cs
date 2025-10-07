using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial15 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Warehouses",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Warehouses",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Status",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Status",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Jobs",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Jobs",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateExpiration",
            table: "Goods",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateManufacture",
            table: "Goods",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "FinalStandards",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "FinalStandards",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "DocumentType2",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "DocumentType2",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "DocumentType1",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "DocumentType1",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserUpdated",
            table: "Documents",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "UserCreated",
            table: "Documents",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "CustomerClassifications",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Purchase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Status = table.Column<bool>(type: "bit", nullable: false),
                Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerClassifications", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RelationShips",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                EmployeeId = table.Column<int>(type: "int", nullable: false),
                EmployeeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                PersonOppositeId = table.Column<int>(type: "int", nullable: false),
                PersonOppositeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ClaimingYourself = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ProclaimedOpposite = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RelationShips", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Relatives",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                BirthDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Facebook = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CompanyId = table.Column<int>(type: "int", nullable: false),
                Gender = table.Column<short>(type: "smallint", nullable: false),
                Status = table.Column<bool>(type: "bit", nullable: false),
                LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                Total = table.Column<double>(type: "float", nullable: false),
                Quit = table.Column<bool>(type: "bit", nullable: false),
                Identify = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                NativePlace = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                PlaceOfPermanent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                IdentifyCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                IdentifyExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Religion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EthnicGroup = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                UnionMember = table.Column<int>(type: "int", nullable: false),
                Nation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                IdentifyCreatedPlace = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Literacy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                LiteracyDetail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Specialize = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Certificate = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Relatives", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CustomerClassifications");

        migrationBuilder.DropTable(
            name: "RelationShips");

        migrationBuilder.DropTable(
            name: "Relatives");

        migrationBuilder.DropColumn(
            name: "DateExpiration",
            table: "Goods");

        migrationBuilder.DropColumn(
            name: "DateManufacture",
            table: "Goods");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Status",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Status",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Jobs",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "FinalStandards",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "DocumentType2",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "DocumentType2",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "DocumentType1",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "DocumentType1",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Documents",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");
    }
}
