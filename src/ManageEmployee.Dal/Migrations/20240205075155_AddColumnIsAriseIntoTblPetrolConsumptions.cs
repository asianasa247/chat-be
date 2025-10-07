using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnIsAriseIntoTblPetrolConsumptions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "PetrolConsumptionPoliceCheckPoints");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "PetrolConsumptionPoliceCheckPoints");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "PetrolConsumptionPoliceCheckPoints");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "PetrolConsumptionPoliceCheckPoints");

        migrationBuilder.AddColumn<bool>(
            name: "IsArise",
            table: "PetrolConsumptionPoliceCheckPoints",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsArise",
            table: "PetrolConsumptionPoliceCheckPoints");

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "PetrolConsumptionPoliceCheckPoints",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "CreatedBy",
            table: "PetrolConsumptionPoliceCheckPoints",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "PetrolConsumptionPoliceCheckPoints",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<int>(
            name: "UpdatedBy",
            table: "PetrolConsumptionPoliceCheckPoints",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }
}
