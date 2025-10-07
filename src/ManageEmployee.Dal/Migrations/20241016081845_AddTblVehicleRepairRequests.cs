using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTblVehicleRepairRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProcedureCodes",
                table: "ProcedureConditions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProcedureConditions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProcedureConditions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProcedureConditions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProcedureCode",
                table: "ProcedureConditions",
                type: "nvarchar(256)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "P_Procedure",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "P_Procedure",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "P_Procedure",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_P_Procedure_Code",
                table: "P_Procedure",
                column: "Code");

            migrationBuilder.CreateTable(
                name: "VehicleRepairRequestDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleRepairRequestId = table.Column<int>(type: "int", nullable: false),
                    GoodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodProducer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodCatalog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GoodType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRepairRequestDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRepairRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserUpdated = table.Column<int>(type: "int", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                    ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRepairRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureConditions_Code_ProcedureCodes",
                table: "ProcedureConditions",
                columns: new[] { "Code", "ProcedureCodes" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [ProcedureCodes] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureConditions_ProcedureCode",
                table: "ProcedureConditions",
                column: "ProcedureCode");

            migrationBuilder.CreateIndex(
                name: "IX_P_Procedure_Code",
                table: "P_Procedure",
                column: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcedureConditions_P_Procedure_ProcedureCode",
                table: "ProcedureConditions",
                column: "ProcedureCode",
                principalTable: "P_Procedure",
                principalColumn: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcedureConditions_P_Procedure_ProcedureCode",
                table: "ProcedureConditions");

            migrationBuilder.DropTable(
                name: "VehicleRepairRequestDetails");

            migrationBuilder.DropTable(
                name: "VehicleRepairRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProcedureConditions_Code_ProcedureCodes",
                table: "ProcedureConditions");

            migrationBuilder.DropIndex(
                name: "IX_ProcedureConditions_ProcedureCode",
                table: "ProcedureConditions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_P_Procedure_Code",
                table: "P_Procedure");

            migrationBuilder.DropIndex(
                name: "IX_P_Procedure_Code",
                table: "P_Procedure");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProcedureConditions");

            migrationBuilder.DropColumn(
                name: "ProcedureCode",
                table: "ProcedureConditions");

            migrationBuilder.AlterColumn<string>(
                name: "ProcedureCodes",
                table: "ProcedureConditions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProcedureConditions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProcedureConditions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "P_Procedure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "P_Procedure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "P_Procedure",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
