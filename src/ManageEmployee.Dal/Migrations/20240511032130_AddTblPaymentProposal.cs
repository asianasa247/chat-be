using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblPaymentProposal : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Warehouses",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Warehouses",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Users",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Users",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Status",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Status",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "StationeryImports",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "StationeryImports",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "StationeryExports",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "StationeryExports",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Stationeries",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Stationeries",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Social",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Social",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Sliders",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Sliders",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "SalaryLevel",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "SalaryLevel",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Relatives",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Relatives",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "RelationShips",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "RelationShips",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "PetrolConsumptions",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "PetrolConsumptions",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "P_SalaryAdvance",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "P_SalaryAdvance",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "P_Leave",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "P_Leave",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "P_Kpis",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "P_Kpis",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "P_Inventories",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "P_Inventories",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "OrderDetail",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "OrderDetail",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Order",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Order",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "News",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "News",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Jobs",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Jobs",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "IsoftHistory",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "IsoftHistory",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "InvoiceDeclarations",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "InvoiceDeclarations",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Introduces",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Introduces",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "GoodWarehouseExport",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "GoodWarehouseExport",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "FinalStandards",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "FinalStandards",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "DocumentTypes",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "DocumentTypes",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "DocumentType2",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "DocumentType2",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "DocumentType1",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "DocumentType1",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Documents",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Documents",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Degrees",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Degrees",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "DecisionType",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "DecisionType",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Decide",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Decide",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "CustomerClassifications",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "CustomerClassifications",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Certificates",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Certificates",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Cart",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Cart",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Cars",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Cars",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Career",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Career",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "AllowanceUsers",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "AllowanceUsers",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "UpdateAt",
            table: "Allowances",
            newName: "UpdatedAt");

        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "Allowances",
            newName: "CreatedAt");

        migrationBuilder.CreateTable(
            name: "AdvancePayments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DatePayment = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdvancePayments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GatePasses",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Local = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GatePasses", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PaymentProposalDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PaymentProposalId = table.Column<int>(type: "int", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Aount = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentProposalDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PaymentProposals",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                PaymentMenthod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TotalAount = table.Column<double>(type: "float", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentProposals", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AdvancePayments");

        migrationBuilder.DropTable(
            name: "GatePasses");

        migrationBuilder.DropTable(
            name: "PaymentProposalDetails");

        migrationBuilder.DropTable(
            name: "PaymentProposals");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Warehouses",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Warehouses",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Users",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Users",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Status",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Status",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "StationeryImports",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "StationeryImports",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "StationeryExports",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "StationeryExports",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Stationeries",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Stationeries",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Social",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Social",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Sliders",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Sliders",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "SalaryLevel",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "SalaryLevel",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Relatives",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Relatives",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "RelationShips",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "RelationShips",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "PetrolConsumptions",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "PetrolConsumptions",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "P_SalaryAdvance",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "P_SalaryAdvance",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "P_Leave",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "P_Leave",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "P_Kpis",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "P_Kpis",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "P_Inventories",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "P_Inventories",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "OrderDetail",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "OrderDetail",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Order",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Order",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "News",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "News",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Jobs",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Jobs",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "IsoftHistory",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "IsoftHistory",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "InvoiceDeclarations",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "InvoiceDeclarations",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Introduces",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Introduces",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "GoodWarehouseExport",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "GoodWarehouseExport",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "FinalStandards",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "FinalStandards",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "DocumentTypes",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "DocumentTypes",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "DocumentType2",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "DocumentType2",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "DocumentType1",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "DocumentType1",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Documents",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Documents",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Degrees",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Degrees",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "DecisionType",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "DecisionType",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Decide",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Decide",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "CustomerClassifications",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "CustomerClassifications",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Certificates",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Certificates",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Cart",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Cart",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Cars",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Cars",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Career",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Career",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "AllowanceUsers",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "AllowanceUsers",
            newName: "CreateAt");

        migrationBuilder.RenameColumn(
            name: "UpdatedAt",
            table: "Allowances",
            newName: "UpdateAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Allowances",
            newName: "CreateAt");
    }
}
