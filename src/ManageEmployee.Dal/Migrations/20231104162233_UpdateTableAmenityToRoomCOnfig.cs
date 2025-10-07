using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Migrations;

public partial class UpdateTableAmenityToRoomCOnfig : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_AmenityTypes",
            table: "AmenityTypes");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Amenities",
            table: "Amenities");

        migrationBuilder.RenameTable(
            name: "AmenityTypes",
            newName: "RoomConfigureTypes");

        migrationBuilder.RenameTable(
            name: "Amenities",
            newName: "RoomConfigures");

        migrationBuilder.RenameColumn(
            name: "RoomType",
            table: "GoodRoomTypes",
            newName: "RoomTypeRoomConfigureId");

        migrationBuilder.RenameColumn(
            name: "AmenityTypeIds",
            table: "GoodRoomTypes",
            newName: "AmenityTypeRoomConfigureyTypeIds");

        migrationBuilder.RenameColumn(
            name: "AmenityIds",
            table: "GoodRoomTypes",
            newName: "AmenityTypeRoomConfigureyIds");

        migrationBuilder.RenameColumn(
            name: "AmenityTypeId",
            table: "RoomConfigures",
            newName: "RoomConfigureTypeId");

        migrationBuilder.AddColumn<int>(
            name: "BedTypeRoomConfigureId",
            table: "GoodRoomTypes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AlterColumn<int>(
            name: "Type",
            table: "RoomConfigureTypes",
            type: "int",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_RoomConfigureTypes",
            table: "RoomConfigureTypes",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_RoomConfigures",
            table: "RoomConfigures",
            column: "Id");

        migrationBuilder.CreateTable(
            name: "GoodRoomBeds",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoomTypeId = table.Column<int>(type: "int", nullable: false),
                AdultQuantity = table.Column<int>(type: "int", nullable: true),
                BedTypeRoomConfigureQuantitys = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodRoomBeds", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GoodRoomBeds");

        migrationBuilder.DropPrimaryKey(
            name: "PK_RoomConfigureTypes",
            table: "RoomConfigureTypes");

        migrationBuilder.DropPrimaryKey(
            name: "PK_RoomConfigures",
            table: "RoomConfigures");

        migrationBuilder.DropColumn(
            name: "BedTypeRoomConfigureId",
            table: "GoodRoomTypes");

        migrationBuilder.RenameTable(
            name: "RoomConfigureTypes",
            newName: "AmenityTypes");

        migrationBuilder.RenameTable(
            name: "RoomConfigures",
            newName: "Amenities");

        migrationBuilder.RenameColumn(
            name: "RoomTypeRoomConfigureId",
            table: "GoodRoomTypes",
            newName: "RoomType");

        migrationBuilder.RenameColumn(
            name: "AmenityTypeRoomConfigureyTypeIds",
            table: "GoodRoomTypes",
            newName: "AmenityTypeIds");

        migrationBuilder.RenameColumn(
            name: "AmenityTypeRoomConfigureyIds",
            table: "GoodRoomTypes",
            newName: "AmenityIds");

        migrationBuilder.RenameColumn(
            name: "RoomConfigureTypeId",
            table: "Amenities",
            newName: "AmenityTypeId");

        migrationBuilder.AlterColumn<string>(
            name: "Type",
            table: "AmenityTypes",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_AmenityTypes",
            table: "AmenityTypes",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Amenities",
            table: "Amenities",
            column: "Id");
    }
}
